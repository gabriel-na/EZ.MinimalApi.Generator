using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Diagnostics;

public static class AttributeDiagnosticRules
{
    private const string StructureCategory = "Structure";
    private const string AttributeUsageCategory = "Attribute Usage";
    private const string IdPrefix = "EZ";

    public static readonly DiagnosticDescriptor GroupNameRequiredRule = new(
        id: $"{IdPrefix}0005",
        title: "Attribute requires GroupName",
        messageFormat: "The attribute '{0}' can only be used on the class '{1}' if the GroupNameAttribute (or equivalent) is present.",
        category: AttributeUsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DotNetVersionNotSupportedRule = new(
        id: $"{IdPrefix}0006",
        title: "Attribute requires .NET 7.0 or higher",
        messageFormat: "Attribute 'GroupNameAttribute' requires .NET 7.0 or higher. The current project is using an earlier version.",
        category: AttributeUsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
