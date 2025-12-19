using System.Collections.Immutable;
using EZ.MinimalApi.Generator.Models;
using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Diagnostics;

internal sealed class DiagnosticResult
{
    public Location Location { get; set; }
    public DiagnosticDescriptor Descriptor { get; set; }
    public string[] MessageParameters { get; set; }
}

internal sealed class TransformResult
{
    public ClassInfo ClassInfo { get; set; }
    public ImmutableArray<DiagnosticResult> Diagnostics { get; set; }
}