using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Models
{
    internal sealed class ClassInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; } = "";
        public string? GroupName { get; set; }
        public ImmutableArray<AttributeData> Attributes { get; set; }
        public List<MethodInfo> Methods { get; set; } = new();
    }
}
