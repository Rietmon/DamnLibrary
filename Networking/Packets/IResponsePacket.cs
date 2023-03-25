#if ENABLE_NETWORKING
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public interface IResponsePacket : ISerializable
    {
        public ResponseType Response { get; set; }
    }

    public static class ResponsePacketExtensions
    {
        public static bool IsValid(this IResponsePacket packet) => packet.Response == ResponseType.Ok;
    }
}
#endif