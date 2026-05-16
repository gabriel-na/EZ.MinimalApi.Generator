using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EndpointGroupNameAttribute : Attribute
{
    public string GroupName { get; }

    public EndpointGroupNameAttribute(string groupName)
    {
        GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
    }
}
