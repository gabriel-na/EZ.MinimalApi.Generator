using System;

namespace EZ.MinimalApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ProducesAttribute : Attribute
    {
        public int StatusCode { get; set; }
        public string? ContentType { get; set; }
        public Type? ResponseType { get; set; }

        public ProducesAttribute(int statusCode, Type responseType = null, string? contentType = null)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            ResponseType = responseType;
        }
    }

    public class ProducesAttribute<T> : ProducesAttribute
    {
        public ProducesAttribute(int statusCode, string? contentType = null)
            : base(statusCode, typeof(T), contentType)
        {
        }
    }
}