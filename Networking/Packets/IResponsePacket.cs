using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public interface IResponsePacket : ISerializable
    {
        public ResponseType Response { get; set; }

        public bool IsValid { get; }
    }
}