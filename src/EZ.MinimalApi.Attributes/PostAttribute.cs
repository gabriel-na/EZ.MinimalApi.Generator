using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a method as handling HTTP POST requests for the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PostAttribute : BaseRouteAttribute
    {
        public PostAttribute(string route) : base(route)
        {
        }
    }
}
