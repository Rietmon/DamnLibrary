using System;

namespace Rietmon.Serialization
{
    public class StaticSerializableAttribute : Attribute
    {
        public short SerializableId { get; }

        public StaticSerializableAttribute(short serializableId)
        {
            SerializableId = serializableId;
        }
    }
}