#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using DamnLibrary.Networking.Client;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Server
{
    public class DamnClientConnection
    {
        private static uint LastClientConnectionId { get; set; }
        
        public uint Id { get; }
        
        public bool IsConnected => ConnectedClient.IsConnected;
        
        public bool IsAvailable => ConnectedClient.IsAvailable;
        
        private DamnClient ConnectedClient { get; }

        public DamnClientConnection(IClientProtocol connectedClientProtocol)
        {
            Id = LastClientConnectionId++;
            ConnectedClient = new DamnClient(connectedClientProtocol);
        }
        
        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(ISerializable sendPacket, IConvertible paketType, params byte[] additionalData)
            where TReceive : ISerializable, new() => 
            await ConnectedClient.SendAsync<TReceive>(sendPacket, paketType, additionalData);

        public async Task SendResponseAsync(PacketHeader receivedPacketHeader, ISerializable sendPacket) =>
            await ConnectedClient.SendResponseAsync(receivedPacketHeader, sendPacket);
        
        public void Disconnect() => ConnectedClient.Disconnect();
    }
}
#endif