using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a method as handling HTTP DELETE requests for the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DeleteAttribute : BaseRouteAttribute
    {
        public DeleteAttribute(string route) : base(route)
        {
        }

        public DeleteAttribute() : this("/")
        {
        }
    }
}
