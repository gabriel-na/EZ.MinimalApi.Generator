using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class DescriptionAttribute : Attribute
{
    public string Text { get; }
    
    public DescriptionAttribute(string text)
    {
        Text = text;
    }
}
