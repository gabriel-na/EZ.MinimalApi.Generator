using System.Collections.Generic;

namespace EZ.MinimalApi.Generator.Models
{
    internal sealed class MethodInfo
    {
        public string MethodName { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public string Route { get; set; } = "";
        public AuthorizationInfo? Authorization { get; set; }
        public List<string> Filters { get; set; } = new();
        public string[]? Tags { get; set; }
        public string? EndpointName { get; set; }
        public bool IsAsync { get; set; }
    }
}
