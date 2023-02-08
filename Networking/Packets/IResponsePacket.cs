using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public interface IResponsePacket : ISerializable
    {
        public ResponseType Response { get; set; }

        public bool IsValid { get; }
        
        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(Response);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            Response = stream.Read<ResponseType>();
        }
    }
}