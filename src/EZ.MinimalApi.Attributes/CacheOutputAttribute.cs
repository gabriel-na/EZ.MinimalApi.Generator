using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CacheOutputAttribute : Attribute
{
    public string? PolicyName { get; }

    public CacheOutputAttribute(string? policyName = null)
    {
        PolicyName = policyName;
    }
}
