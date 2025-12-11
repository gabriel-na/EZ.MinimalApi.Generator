using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Define o nome do endpoint
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public string EndpointName { get; }

        public NameAttribute(string endpointName)
        {
            EndpointName = endpointName ?? throw new ArgumentNullException(nameof(endpointName));
        }
    }
}
