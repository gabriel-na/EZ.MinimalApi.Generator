using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EnableRateLimitingAttribute : Attribute
{
    public string? PolicyName { get; }

    public EnableRateLimitingAttribute(string? policyName = null)
    {
        PolicyName = policyName;
    }
}
