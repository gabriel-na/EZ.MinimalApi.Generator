using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SummaryAttribute : Attribute
{
    public string Text { get; }

    public SummaryAttribute(string text)
    {
        Text = text;
    }
}
