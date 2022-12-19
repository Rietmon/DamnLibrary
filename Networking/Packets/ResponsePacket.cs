#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public struct ResponsePacket : ISerializable
    {
        public ResponseType Response { get; set; }
        
        public void Serialize(SerializationStream stream)
        {
            stream.Write(Response);
        }

        public void Deserialize(SerializationStream stream)
        {
            Response = stream.Read<ResponseType>();
        }
    }

    public enum ResponseType : byte
    {
        Invalid,
        Ok,
        No,
        Error
    }
}
#endif