using System;

namespace EZ.MinimalApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ProducesProblemAttribute : Attribute
    {
        public int StatusCode { get; }
        public Type? ResponseType { get; }
        public string? ContentType { get; }

        public ProducesProblemAttribute(int statusCode, Type? responseType = null, string? contentType = null)
        {
            StatusCode = statusCode;
            ResponseType = responseType;
            ContentType = contentType;
        }
    }

    public class ProducesProblemAttribute<TResponse> : ProducesProblemAttribute
    {
        public ProducesProblemAttribute(int statusCode, string? contentType = null)
            : base(statusCode, typeof(TResponse), contentType)
        {
        }
    }
}
