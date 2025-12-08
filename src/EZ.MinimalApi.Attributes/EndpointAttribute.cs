using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a class as an API endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EndpointAttribute : Attribute
    { }
}
