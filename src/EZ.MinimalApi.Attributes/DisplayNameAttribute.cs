using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class DisplayNameAttribute : Attribute
{
    public string Text { get; }
    
    public DisplayNameAttribute(string text)
    {
        Text = text;
    }
}
