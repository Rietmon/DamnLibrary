using System;
using System.Threading.Tasks;
using DamnLibrary.Networking.Client;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Networking.Protocols.TCP;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking
{
    public class DamnServerClientConnection
    {
        public bool IsConnected => ConnectedClient.IsConnected;
        
        public bool IsAvailable => ConnectedClient.IsAvailable;
        
        private DamnClient ConnectedClient { get; }

        public DamnServerClientConnection(IClientProtocol connectedClientProtocol)
        {
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