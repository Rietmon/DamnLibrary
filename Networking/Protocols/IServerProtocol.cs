#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Threading.Tasks;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Server;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Protocols
{
    public interface IServerProtocol
    {
        public Func<Task> OnHandle { get; set; }
        public Action<ServerConnection>  OnAcceptConnection { get; set; }
        public Action<ServerConnection> OnRejectingConnection { get; set; }
        public Action OnRejectConnection { get; set; }
        public Action OnUpdatedConnections { get; set; }
        
        public bool IsWorking { get; }
        
        public bool IsPaused { get; }

        void Handle();

        int GetServerConnectionsCount();
        
        ServerConnection GetServerConnection(uint id);
        
        Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(ServerConnection serverConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<ServerConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData)
            where TReceive : ISerializable, new();

        public Task SendWithoutResponseAsync(ServerConnection clientConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData);

        public Task SendToSelectedWithoutResponseAsync(Func<ServerConnection, bool> predicate,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData);

        public Task SendToEachWithoutResponseAsync(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData);

        void Stop();
    }
}
#endif