using System;

namespace EZ.MinimalApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class FallbackAttribute : BaseRouteAttribute
    {
        public FallbackAttribute(string route) : base(route)
        {
        }

        public FallbackAttribute() : this("/")
        {
        }
    }
}
