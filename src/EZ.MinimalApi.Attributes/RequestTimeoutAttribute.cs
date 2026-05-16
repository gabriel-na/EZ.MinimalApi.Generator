using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequestTimeoutAttribute : Attribute
{
    public int Milliseconds { get; }
    public string? PolicyName { get; }

    public RequestTimeoutAttribute(int milliseconds)
    {
        Milliseconds = milliseconds;
    }

    public RequestTimeoutAttribute(string policyName)
    {
        PolicyName = policyName;
    }
}
