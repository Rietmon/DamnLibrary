#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public struct EmptyPacket : ISerializable
    {
        public void Serialize(SerializationStream stream) { }

        public void Deserialize(SerializationStream stream) { }
    }
}
#endif