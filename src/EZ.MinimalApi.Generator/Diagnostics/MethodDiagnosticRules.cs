using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Diagnostics;

public static class MethodDiagnosticRules
{
    private const string Category = "Structure";
    private const string IdPrefix = "EZ";

    public static readonly DiagnosticDescriptor MethodMustBeStaticRule = new(
        id: $"{IdPrefix}0003",
        title: "Method must be static",
        messageFormat: "The method '{0}' must be declared as 'static' to be registered as an endpoint",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MethodMustHaveBodyRule = new(
        id: $"{IdPrefix}0004",
        title: "Invalid endpoint method",
        messageFormat: "The method '{0}' must have a block body or be an expression body",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MethodMustHaveOnlyOneAttributeRule = new(
        id: $"{IdPrefix}0007",
        title: "Multiple HTTP method attributes found",
        messageFormat: "The method '{0}' must have only one HTTP method attribute",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

}
