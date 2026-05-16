using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequestSizeLimitAttribute : Attribute
{
    public int Bytes { get; }

    public RequestSizeLimitAttribute(int bytes)
    {
        Bytes = bytes;
    }
}
