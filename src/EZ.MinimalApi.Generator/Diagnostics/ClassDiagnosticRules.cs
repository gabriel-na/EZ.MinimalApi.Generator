using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Diagnostics;

public static class ClassDiagnosticRules
{
    private const string Category = "Structure";
    private const string IdPrefix = "EZ";

    public static readonly DiagnosticDescriptor ClassMustBeStaticRule = new(
        id: $"{IdPrefix}0001",
        title: "Class must be static",
        messageFormat: "The class '{0}' that contains [Endpoint] must be declared as 'static'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ClassIsEmptyRule = new(
        id: $"{IdPrefix}0002",
        title: "Class has no endpoint methods",
        messageFormat: "The class '{0}' does not contain any annotated methods and will be ignored",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
