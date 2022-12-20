#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Threading.Tasks;
using DamnLibrary.Networking.Client;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Networking.Protocols.TCP;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Server
{
    public class ServerConnection
    {
        private static uint LastServerConnectionId { get; set; }
        
        public uint Id { get; }
        
        public Action OnConnect { get => ConnectedClient.OnConnect; set => ConnectedClient.OnConnect = value; }
        
        public Action<NetworkPacket> OnPacketReceive { get => ConnectedClient.OnPacketReceive; set => ConnectedClient.OnPacketReceive = value; }

        public Action OnDisconnect { get => ConnectedClient.OnDisconnect; set => ConnectedClient.OnDisconnect = value; }
        
        public bool IsConnected => ConnectedClient.IsConnected;
        
        public bool IsPaused { get => ConnectedClient.IsPaused; set => ConnectedClient.IsPaused = value; }
        
        private DamnClient ConnectedClient { get; }

        public ServerConnection(IClientProtocol connectedClientProtocol)
        {
            Id = LastServerConnectionId++;
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