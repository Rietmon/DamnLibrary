using System;

namespace Tiny
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonPropertyAttribute : Attribute
    {
        public string Name { get; }

        public JsonPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}