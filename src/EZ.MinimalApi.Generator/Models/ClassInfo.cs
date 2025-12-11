using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EZ.MinimalApi.Attributes;
using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Models
{
    internal sealed class ClassInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; } = "";
        public ImmutableArray<AttributeData> Attributes { get; set; }
        public List<MethodInfo> Methods { get; set; } = new();

        internal string? GetGroupName()
        {
            var groupAttribute = Attributes.FirstOrDefault(a => a.AttributeClass?.Name == nameof(GroupEndpointAttribute));

            return groupAttribute?.ConstructorArguments[0].Value?.ToString() ?? null;
        }
    }
}
