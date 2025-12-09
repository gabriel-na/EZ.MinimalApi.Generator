using System;

namespace EZ.MinimalApi.Attributes
{
    public abstract class BaseRouteAttribute : Attribute
    {
        public string Route { get; }

        public BaseRouteAttribute(string route)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
        }
    }
}
