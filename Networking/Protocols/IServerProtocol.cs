#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Threading;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Server;
using DamnLibrary.Other;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Protocols
{
    public interface IServerProtocol
    {
        public bool IsWorking { get; }
        
        public bool IsPaused { get; }

        void Handle();
        
        public DamnClientConnection GetClientConnection(uint id);

        public Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(int clientConnectionId, ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        public Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(DamnClientConnection clientConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        public Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<DamnClientConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        public Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData)
            where TReceive : ISerializable, new();

        void Stop();
    }
}
#endif