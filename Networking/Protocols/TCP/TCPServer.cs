#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Server;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Protocols.TCP
{
    public class TCPServer : ServerNetworkHandler, IServerProtocol
    {
        public override bool IsWorking { get; set; }

        public override bool IsPaused { get; set; }

        private TcpListener Server { get; set; }

        private List<DamnClientConnection> ClientConnections { get; } = new();

        public TCPServer(TcpListener server)
        {
            Server = server;

            Server.Start();
            IsWorking = true;
        }

        public DamnClientConnection GetClientConnection(uint id) =>
            ClientConnections.FirstOrDefault((connection) => connection.Id == id);

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(int clientConnectionId,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
            => await ClientConnections[clientConnectionId].SendAsync<TReceive>(sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(DamnClientConnection clientConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
            => await clientConnection.SendAsync<TReceive>(sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<DamnClientConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var connectionsToSend = ClientConnections.Where(predicate).ToArray();
            var responses = new Pair<PacketHeader, TReceive>[connectionsToSend.Length];
            
            for (var i = 0; i < connectionsToSend.Length; i++)
            {
                responses[i] = await connectionsToSend[i].SendAsync<TReceive>(sendPacket, packetType, additionalData);
            }

            return responses;
        }

        public async Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var responses = new Pair<PacketHeader, TReceive>[ClientConnections.Count];
            for (var i = 0; i < ClientConnections.Count; i++)
            {
                responses[i] = await ClientConnections[i].SendAsync<TReceive>(sendPacket, packetType, additionalData);
            }

            return responses;
        }

        public void Stop()
        {
            IsWorking = false;
            Server.Stop();

            while (ClientConnections.Count > 0)
            {
                ClientConnections[0].Disconnect();
                ClientConnections.RemoveAt(0);
            }
        }

        protected override async Task OnHandleAsync()
        {
            var connection = await Server.AcceptTcpClientAsync();
            var clientConnection = new DamnClientConnection(new TCPClient(connection, false));
            ClientConnections.Add(clientConnection);
        }
    }
}
#endif