using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class CorsAttribute : Attribute
{
    public string? PolicyName { get; }

    public CorsAttribute(string? policyName = null)
    {
        PolicyName = policyName;
    }
}
