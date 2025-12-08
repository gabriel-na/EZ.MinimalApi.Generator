using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a method as handling HTTP GET requests for the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GetAttribute : BaseRouteAttribute
    {
        public GetAttribute(string route) : base(route)
        {
        }
    }
}
