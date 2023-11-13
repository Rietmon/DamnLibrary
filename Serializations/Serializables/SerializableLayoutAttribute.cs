#if ENABLE_SERIALIZATION
using System;

namespace DamnLibrary.Serializations.Serializables
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SerializableLayoutAttribute : Attribute
    {
        public bool WrapToContainer { get; set; }
        public bool UseKeyValuePair { get; set; }
    }
}
#endif