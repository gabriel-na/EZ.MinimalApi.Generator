using EZ.MinimalApi.Attributes;
using EZ.MinimalApi.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EZ.MinimalApi.Generator
{
    [Generator]
    public class EndpointSourceGenerator : IIncrementalGenerator
    {
        private static readonly SymbolDisplayFormat FullyQualifiedWithoutGlobal = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                transform: static (ctx, _) => TransformClass(ctx))
                .Where(static x => x is not null);

            context.RegisterSourceOutput(provider.Collect(), (spc, classes) =>
            {
                var list = classes.Where(c => c != null).Cast<ClassInfo>().ToList();

                if (list.Count == 0)
                    return;

                GenerateFiles(spc, list);
                GenerateAggregator(spc, list);
            });
        }

        private static ClassInfo? TransformClass(GeneratorSyntaxContext ctx)
        {
            var classDeclaration = (ClassDeclarationSyntax)ctx.Node;
            var semanticModel = ctx.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (classSymbol == null || !classSymbol.IsStatic)
                return null;

            var hasEndpoint = classSymbol.GetAttributes()
                .Any(a => a.AttributeClass?.Name == nameof(EndpointAttribute));

            if (!hasEndpoint)
                return null;

            var groupAttribute = classSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GroupEndpointAttribute));

            string? groupName = null;

            if (groupAttribute != null && groupAttribute.ConstructorArguments.Length > 0)
                groupName = groupAttribute.ConstructorArguments[0].Value?.ToString();

            var methods = new List<MethodInfo>();

            foreach (var methodDeclaration in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;

                if (methodSymbol == null)
                    continue;

                var methodInfo = GetMethodInfo(methodSymbol, methodDeclaration);

                if (methodInfo != null)
                    methods.Add(methodInfo);
            }

            if (methods.Count == 0)
                return null;

            return new ClassInfo
            {
                Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
                Name = classSymbol.Name,
                GroupName = groupName,
                Methods = methods,
                Attributes = classSymbol.GetAttributes()
            };
        }

        private static MethodInfo? GetMethodInfo(IMethodSymbol methodSymbol, MethodDeclarationSyntax methodDeclaration)
        {
            var methodAttributes = methodSymbol.GetAttributes();

            var httpAttributes = methodAttributes.FirstOrDefault(a =>
                a.AttributeClass?.Name == nameof(GetAttribute) ||
                a.AttributeClass?.Name == nameof(PostAttribute) ||
                a.AttributeClass?.Name == nameof(PutAttribute) ||
                a.AttributeClass?.Name == nameof(PatchAttribute) ||
                a.AttributeClass?.Name == nameof(DeleteAttribute)
            );

            if (httpAttributes == null)
                return null;

            var httpMethod = httpAttributes.AttributeClass!.Name.Replace("Attribute", "");
            var route = "/";

            if (httpAttributes.ConstructorArguments.Length > 0)
                route = httpAttributes.ConstructorArguments[0].Value?.ToString() ?? "/";

            var authInfo = GetAuthorizationInfo(methodAttributes);
            var filters = GetFilters(methodAttributes);
            var tags = GetTags(methodAttributes);
            var endpointName = GetName(methodAttributes);

            var hasBody = methodDeclaration.Body != null || methodDeclaration.ExpressionBody != null;

            if (!hasBody)
                return null;

            return new MethodInfo
            {
                MethodName = methodSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                HttpMethod = httpMethod,
                Route = route,
                Authorization = authInfo,
                Filters = filters,
                Tags = tags,
                EndpointName = endpointName,
                IsAsync = methodSymbol.IsAsync,
            };
        }

        private static AuthorizationInfo? GetAuthorizationInfo(ImmutableArray<AttributeData> attributes)
        {
            var authorizationAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == nameof(AuthorizationAttribute));

            if (authorizationAttribute == null)
                return null;

            var authorizationInfo = new AuthorizationInfo();

            if (authorizationAttribute.ConstructorArguments.Length > 0)
            {
                var argument = authorizationAttribute.ConstructorArguments[0];

                if (argument.Kind == TypedConstantKind.Array && argument.Type?.ToDisplayString() == "string[]")
                {
                    authorizationInfo.Mode = AuthorizationMode.PolicyNames;
                    authorizationInfo.PolicyNames = argument.Values.Select(v => v.Value?.ToString()).Where(s => s != null).Cast<string>().ToArray(); ;

                    return authorizationInfo;
                }

                if (argument.Value is string policyName)
                {
                    authorizationInfo.Mode = AuthorizationMode.PolicyNames;
                    authorizationInfo.PolicyNames = new[] { policyName };

                    return authorizationInfo;
                }

                if (argument.Kind == TypedConstantKind.Type)
                {
                    var policyType = argument.Value as INamedTypeSymbol;
                    authorizationInfo.Mode = AuthorizationMode.AuthorizationPolicy;
                    authorizationInfo.AuthorizationPolicyType = policyType?.ToDisplayString(FullyQualifiedWithoutGlobal);

                    return authorizationInfo;
                }

                if (argument.Kind == TypedConstantKind.Array && argument.Values.Length > 0 && argument.Values[0].Kind == TypedConstantKind.Type)
                {
                    var types = argument.Values.Select(v => v.Value as INamedTypeSymbol).Where(t => t != null).Select(t => t!.ToDisplayString(FullyQualifiedWithoutGlobal)).ToArray();
                    authorizationInfo.Mode = AuthorizationMode.AuthorizeData;
                    authorizationInfo.AuthorizeDataTypes = types;

                    return authorizationInfo;
                }
            }

            return authorizationInfo;
        }

        private static List<string> GetFilters(ImmutableArray<AttributeData> attributes)
        {
            return attributes
                .Where(a => a.AttributeClass?.MetadataName.StartsWith("Filter") ?? false)
                .Select(a =>
                {
                    if (a.ConstructorArguments.Length > 0 && a.ConstructorArguments[0].Kind == TypedConstantKind.Type)
                    {
                        var typeSymbol = a.ConstructorArguments[0].Value as INamedTypeSymbol;
                        return typeSymbol?.ToDisplayString(FullyQualifiedWithoutGlobal);
                    }

                    if (a.AttributeClass?.IsGenericType == true)
                    {
                        var typeArgument = a.AttributeClass.TypeArguments.FirstOrDefault();

                        return (typeArgument as INamedTypeSymbol)?.ToDisplayString(FullyQualifiedWithoutGlobal);
                    }
                    return null;
                })
                .Where(s => s != null)
                .Cast<string>()
                .ToList();
        }

        private static string[]? GetTags(ImmutableArray<AttributeData> attributes)
        {
            var tagAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == nameof(TagsAttribute));

            if (tagAttribute != null && tagAttribute.ConstructorArguments.Length > 0)
            {
                var first = tagAttribute.ConstructorArguments[0];

                if (first.Kind == TypedConstantKind.Array)
                    return first.Values.Select(v => v.Value?.ToString()).Where(v => v != null).Cast<string>().ToArray();
                else
                {
                    var single = first.Value?.ToString();

                    if (single != null)
                        return new[] { single };
                }
            }

            return null;
        }

        private static string? GetName(ImmutableArray<AttributeData> attributes)
        {
            var nameAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "NameAttribute");

            return nameAttribute != null && nameAttribute.ConstructorArguments.Length > 0 ?
                nameAttribute.ConstructorArguments[0].Value?.ToString() :
                null;
        }

        private static void GenerateFiles(SourceProductionContext context, List<ClassInfo> classes)
        {
            foreach (var @class in classes)
            {
                using var sw = new StringWriter();
                using var writer = new IndentedTextWriter(sw, "\t");

                writer.WriteLine("using Microsoft.AspNetCore.Builder;");
                writer.WriteLine("using Microsoft.AspNetCore.Http;");
                writer.WriteLine("using Microsoft.AspNetCore.Authorization;");
                writer.WriteLine();
                writer.WriteLine($"namespace EZ.MinimalApi.GeneratedEndpoints");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine($"public static class {@class.Name}Extensions");
                writer.WriteLine("{");
                writer.Indent++;

                writer.WriteLine($"public static WebApplication Map{@class.Name}(this WebApplication app)");
                writer.WriteLine("{");
                writer.Indent++;

                var hasGroup = !string.IsNullOrEmpty(@class.GroupName);
                var root = hasGroup ? "group" : "app";

                if (hasGroup)
                {
                    writer.WriteLine($"var group = app.MapGroup(\"{EscapeString(@class.GroupName)}\");");

                    var classFilters = GetFilters(@class.Attributes);

                    foreach (var filter in classFilters)
                        writer.WriteLine($"group = group.AddEndpointFilter<{filter}>();");

                    writer.WriteLine();
                }

                foreach (var method in @class.Methods)
                {
                    var handler = $"{@class.Namespace}.{@class.Name}.{method.MethodName}" ;

                    writer.WriteLine($"{root}.Map{method.HttpMethod}(\"{EscapeString(method.Route)}\", {handler})");
                    writer.Indent++;
                    
                    AppendAuthorization(writer, method);
                    AppendFilters(writer, method);
                    AppendTags(writer, method);
                    AppendName(writer, method);
                    
                    writer.Write(";");
                    writer.Indent--;
                    writer.WriteLine();
                }

                writer.WriteLine("return app;");
                writer.Indent--;
                writer.WriteLine("}");
                writer.Indent--;
                writer.WriteLine("}");
                writer.Indent--;
                writer.WriteLine("}");

                var generatedClass = sw.ToString();
                context.AddSource($"{@class.Name}_Endpoints.g.cs", SourceText.From(generatedClass, Encoding.UTF8));
            }
        }

        private static void GenerateAggregator(SourceProductionContext context, List<ClassInfo> classes)
        {
            using var sw = new StringWriter();
            using var writer = new IndentedTextWriter(sw, "\t");
            writer.WriteLine("using Microsoft.AspNetCore.Builder;");
            writer.WriteLine();
            writer.WriteLine("namespace EZ.MinimalApi.Extensions");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("public static class WebApplicationAggregatorExtensions");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine("public static WebApplication MapCustomEndpoints(this WebApplication app)");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var @class in classes)
                writer.WriteLine($"EZ.MinimalApi.GeneratedEndpoints.{@class.Name}Extensions.Map{@class.Name}Endpoints(app);");

            writer.WriteLine();
            writer.WriteLine("return app;");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");

            context.AddSource("WebApplicationAggregatorExtensions.g.cs", SourceText.From(sw.ToString(), Encoding.UTF8));
        }

        private static void AppendAuthorization(IndentedTextWriter writer, MethodInfo method)
        {
            var authorization = method.Authorization;
            if (authorization == null) return;

            switch (authorization.Mode)
            {
                case AuthorizationMode.PolicyNames:
                    if (authorization.PolicyNames != null && authorization.PolicyNames.Length == 1)
                    {
                        writer.Write($".RequireAuthorization(\"{EscapeString(authorization.PolicyNames[0])}\")");
                    }
                    else if (authorization.PolicyNames != null && authorization.PolicyNames.Length > 1)
                    {
                        var joined = string.Join("\", \"", authorization.PolicyNames.Select(EscapeString));
                        writer.Write($".RequireAuthorization(new[] {{ \"{joined}\" }})");
                    }
                    break;

                case AuthorizationMode.AuthorizationPolicy:
                    if (!string.IsNullOrEmpty(authorization.AuthorizationPolicyType))
                        writer.Write($".RequireAuthorization({authorization.AuthorizationPolicyType}.Instance)");
                    else
                        writer.Write("//.RequireAuthorization(...) -> AuthorizationPolicy could not be resolved at compile-time");

                    break;

                case AuthorizationMode.AuthorizeData:
                    if (authorization.AuthorizeDataTypes != null && authorization.AuthorizeDataTypes.Length > 0)
                    {
                        var items = string.Join(", ", authorization.AuthorizeDataTypes.Select(t => $"new {t}()"));
                        writer.Write($".RequireAuthorization(new IAuthorizeData[] {{ {items} }})");
                    }
                    else
                        writer.Write("//.RequireAuthorization(...) -> authorize data could not be resolved");

                    break;
            }
        }

        private static void AppendFilters(IndentedTextWriter writer, MethodInfo method)
        {
            foreach (var filter in method.Filters)
                writer.WriteLine($".AddEndpointFilter<{filter}>()");
        }

        private static void AppendTags(IndentedTextWriter writer, MethodInfo method)
        {
            if (method.Tags != null && method.Tags.Length > 0)
            {
                var tagsStr = string.Join("\", \"", method.Tags.Select(EscapeString));
                writer.WriteLine($".WithTags(\"{tagsStr}\")");
            }
        }

        private static void AppendName(IndentedTextWriter writer, MethodInfo method)
        {
            if (!string.IsNullOrEmpty(method.EndpointName))
            {
                writer.WriteLine($".WithName(\"{EscapeString(method.EndpointName)}\")");
            }
        }

        private static string EscapeString(string input) =>
            input?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
    }
}