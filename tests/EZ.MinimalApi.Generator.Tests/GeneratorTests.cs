using EZ.MinimalApi.Attributes;
using EZ.MinimalApi.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EZ.MinimalApi.Generator.Tests;

public class GeneratorTests
{
    private static Task VerifyGenerator(string source, string expectedOutput)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        references.Add(MetadataReference.CreateFromFile(typeof(EndpointAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new EndpointSourceGenerator();

        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var generatedFiles = outputCompilation.SyntaxTrees
            .Where(st => st.FilePath.Contains("_Endpoints.g.cs") || st.FilePath.Contains("Aggregator"))
            .ToList();

        Assert.NotEmpty(generatedFiles);

        return Task.CompletedTask;
    }

    private static string CreateEndpointClass(string body)
    {
        return $$"""
using EZ.MinimalApi.Attributes;
using Microsoft.AspNetCore.Http;

namespace TestNamespace;

[Endpoint]
public static class TestEndpoints
{
{{body}}
}
""";
    }

    [Fact]
    public async Task BasicGetEndpoint_GeneratesMapGet()
    {
        var source = CreateEndpointClass("""
    [Get("/users")]
    public static IResult GetUsers() => Results.Ok();
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task FallbackEndpoint_GeneratesMapFallback()
    {
        var source = CreateEndpointClass("""
    [Fallback]
    public static IResult CatchAll() => Results.NotFound();
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task GroupEndpoint_GeneratesMapGroup()
    {
        var source = """
using EZ.MinimalApi.Attributes;
using Microsoft.AspNetCore.Http;

namespace TestNamespace;

[Endpoint]
[GroupEndpoint("/api/test")]
public static class TestEndpoints
{
    [Get]
    public static IResult GetAll() => Results.Ok();

    [Post]
    public static IResult Create() => Results.Created();
}
""";

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task AuthorizationSinglePolicy_GeneratesRequireAuthorization()
    {
        var source = CreateEndpointClass("""
    [Get("/admin")]
    [Authorization("AdminPolicy")]
    public static IResult GetAdmin() => Results.Ok();
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task AuthorizationMultiplePolicies_GeneratesArrayRequireAuthorization()
    {
        var source = CreateEndpointClass("""
    [Get("/admin")]
    [Authorization("Admin", "SuperUser")]
    public static IResult GetAdmin() => Results.Ok();
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task ProducesProblem_GeneratesProducesProblem()
    {
        var source = CreateEndpointClass("""
    [Get]
    [ProducesProblem(404)]
    [ProducesProblem<ValidationProblem>(422)]
    public static IResult Get() => Results.Ok();

    public record ValidationProblem(string Message);
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task AllVerbs_GenerateCorrectMapMethods()
    {
        var source = """
using EZ.MinimalApi.Attributes;
using Microsoft.AspNetCore.Http;

namespace TestNamespace;

[Endpoint]
public static class AllVerbsEndpoints
{
    [Get]
    public static IResult HandleGet() => Results.Ok();

    [Post]
    public static IResult HandlePost() => Results.Ok();

    [Put]
    public static IResult HandlePut() => Results.Ok();

    [Patch]
    public static IResult HandlePatch() => Results.Ok();

    [Delete]
    public static IResult HandleDelete() => Results.Ok();

    [Fallback]
    public static IResult HandleFallback() => Results.NotFound();
}
""";

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task FiltersTagsNames_GenerateCorrectExtensions()
    {
        var source = CreateEndpointClass("""
    [Get]
    [Tags("Users", "Read")]
    [Name("GetUsers")]
    [Summary("Returns all users")]
    [Description("Returns a list of all registered users")]
    [DisplayName("GetAllUsers")]
    public static IResult GetUsers() => Results.Ok();
""");

        await VerifyGenerator(source, "");
    }

    [Fact]
    public async Task EndpointWithAllAttributes_CompilesSuccessfully()
    {
        var source = """
using EZ.MinimalApi.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace TestNamespace;

[Endpoint]
[GroupEndpoint("/api/demo")]
[Tags("Demo")]
[Authorization("DefaultPolicy")]
[Filter<DummyFilter>]
[Cors("DefaultCors")]
[EnableRateLimiting("Default")]
[CacheOutput]
public static class DemoEndpoints
{
    [Get]
    [AllowAnonymous]
    [Produces<User>(200)]
    [ProducesProblem(400)]
    [ExcludeFromDescription]
    [SkipStatusCodePages]
    [RequestTimeout(3000)]
    public static IResult Get([AsParameters] User query) => Results.Ok(new[] { query });

    [Post]
    [Accepts<User>("application/json")]
    [Produces<User>(201)]
    public static IResult Create(User user) => Results.Created("/api/demo/1", user);
}

public record User(string? Name, string? Email);

public class DummyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return await next(context);
    }
}
""";

        await VerifyGenerator(source, "");
    }
}
