using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AcceptsAttribute : Attribute
{
    public Type RequestType { get; }
    public string ContentType { get; set; }
    public string[] AddtionalContentTypes { get; set; }

    public AcceptsAttribute(Type requestType, string contentType, params string[] addtionalContentTypes)
    {
        RequestType = requestType;
        ContentType = contentType;
        AddtionalContentTypes = addtionalContentTypes;
    }
}

public class AcceptsAttribute<TRequest> : AcceptsAttribute
{
    public AcceptsAttribute(string contentType, params string[] addtionalContentTypes)
        : base(typeof(TRequest), contentType, addtionalContentTypes)
    {
    }
}
