using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class CorsAttribute : Attribute
{
    public string[] PolicyNames { get; }

    public CorsAttribute()
    {
        PolicyNames = Array.Empty<string>();
    }

    public CorsAttribute(params string[] policyNames)
    {
        PolicyNames = policyNames ?? Array.Empty<string>();
    }
}
