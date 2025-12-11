using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace EZ.MinimalApi.Generator.Models
{
    internal sealed class MethodInfo
    {
        public string MethodName { get; set; }
        public string HttpMethod { get; set; }
        public string Route { get; set; }
        public ImmutableArray<AttributeData> Attributes { get; set; }
    }
}
