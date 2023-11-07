using System;

namespace DamnLibrary.TinyJson
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