using System;

namespace DamnLibrary.Serializations.Serializables
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SerializeLayoutAttribute : Attribute
    {
        public bool WrapToContainer { get; set; }
        public bool UseKeyValuePair { get; set; }
    }
}