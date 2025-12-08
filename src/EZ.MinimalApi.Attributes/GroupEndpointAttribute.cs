using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Marks a class as belonging to a specific API group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GroupEndpointAttribute : Attribute
    {
        public string GroupName { get; }

        public GroupEndpointAttribute(string groupName)
        {
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
        }
    }
}
