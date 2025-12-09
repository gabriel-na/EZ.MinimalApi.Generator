namespace EZ.MinimalApi.Generator.Models
{
    internal sealed class AuthorizationInfo
    {
        public AuthorizationMode Mode { get; set; }

        public string[]? PolicyNames { get; set; }

        public string? AuthorizationPolicyType { get; set; }

        public string[]? AuthorizeDataTypes { get; set; }
    }
}