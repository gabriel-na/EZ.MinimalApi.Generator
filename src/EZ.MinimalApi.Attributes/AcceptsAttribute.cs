using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AcceptsAttribute : Attribute
{
    public Type RequestType { get; }
    public string ContentType { get; set; }
    public string[] AdditionalContentTypes { get; set; }

    public AcceptsAttribute(Type requestType, string contentType, params string[] additionalContentTypes)
    {
        RequestType = requestType;
        ContentType = contentType;
        AdditionalContentTypes = additionalContentTypes;
    }
}

public class AcceptsAttribute<TRequest> : AcceptsAttribute
{
    public AcceptsAttribute(string contentType, params string[] additionalContentTypes)
        : base(typeof(TRequest), contentType, additionalContentTypes)
    {
    }
}
