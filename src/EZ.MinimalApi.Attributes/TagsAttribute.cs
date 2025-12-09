using System;

namespace EZ.MinimalApi.Attributes
{
    /// <summary>
    /// Adiciona tags ao endpoint
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TagsAttribute : Attribute
    {
        public string[] Tags { get; }

        public TagsAttribute(params string[] tags)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }
    }
}
