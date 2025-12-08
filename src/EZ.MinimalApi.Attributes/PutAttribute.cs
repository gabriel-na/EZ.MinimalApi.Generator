using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a method as handling HTTP PUT requests for the specified route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PutAttribute : BaseRouteAttribute
    {
        public PutAttribute(string route) : base(route)
        {
        }
    }
}
