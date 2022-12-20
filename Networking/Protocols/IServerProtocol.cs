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
        public Action<DamnClientConnection>  OnAcceptConnection { get; set; }
        public Action<DamnClientConnection> OnRejectingConnection { get; set; }
        public Action OnRejectConnection { get; set; }
        
        public bool IsWorking { get; }
        
        public bool IsPaused { get; }

        void Handle();
        
        DamnClientConnection GetClientConnection(uint id);

        Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(int clientConnectionId, ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(DamnClientConnection clientConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<DamnClientConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new();
        Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData)
            where TReceive : ISerializable, new();

        void Stop();
    }
}
#endif