using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a method as handling HTTP PATCH requests for the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PatchAttribute : BaseRouteAttribute
    {
        public PatchAttribute(string route) : base(route)
        {
        }
    }
}
