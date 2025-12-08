using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Adiciona um filtro ao endpoint
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class FilterAttribute : Attribute
    {
        public Type FilterType { get; }

        public FilterAttribute(Type filterType)
        {
            FilterType = filterType ?? throw new ArgumentNullException(nameof(filterType));
        }
    }

    public class FilterAttribute<TFilter> : FilterAttribute
    {
        public FilterAttribute() : base(typeof(TFilter))
        {
        }
    }
}
