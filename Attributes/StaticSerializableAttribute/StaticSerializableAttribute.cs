#if ENABLE_SERIALIZATION
using System;

namespace Rietmon.Serialization
{
    /// <summary>
    /// Marked class as static serializable.
    /// Use SerializationManager.SerializeStaticTypes/DeserializeStaticTypes to fast serializing a few static classes
    /// </summary>
    public class StaticSerializableAttribute : Attribute
    {
        /// <summary>
        /// Id for this class, use unique ids for each static classes! 
        /// </summary>
        public short SerializableId { get; }
        
        public StaticSerializableAttribute(short serializableId)
        {
            SerializableId = serializableId;
        }
    }
}
#endif