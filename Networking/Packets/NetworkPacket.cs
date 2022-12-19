#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public class NetworkPacket : IDisposable
    {
        public PacketHeader Header { get; }
        
        public SerializationStream DeserializationStream { get; }
        
        public bool IsHandled { get; set; }

        public NetworkPacket(PacketHeader header, SerializationStream deserializationStream)
        {
            Header = header;
            DeserializationStream = deserializationStream;
        }

        public void Dispose()
        {
            DeserializationStream.Dispose();
        }
    }
}
#endif