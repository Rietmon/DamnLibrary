#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System.Net;
using System.Net.Sockets;
using DamnLibrary.Debugging;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Networking.Protocols.TCP;
using DamnLibrary.Other;
using DamnLibrary.Serialization;
using ProtocolType = DamnLibrary.Networking.Protocols.ProtocolType;

namespace DamnLibrary.Networking.Server
{
    public class DamnServer
    {
        public bool IsWorking => Server.IsWorking;
        
        public bool IsPaused => Server.IsPaused;
        
        private IServerProtocol Server { get; set; }

        public void Start(ProtocolType protocolType, string address, int port)
        {
            if (Server != null)
            {
                UniversalDebugger.LogError($"[{nameof(DamnServer)}] ({nameof(Start)}) Server is already started");
                return;
            }
            
            switch (protocolType)
            {
                case ProtocolType.TCP: Server = new TCPServer(new TcpListener(IPAddress.Parse(address), port)); break;
                default: UniversalDebugger.LogError($"[{nameof(DamnServer)}] ({nameof(Start)}) Unknown protocol type, type = {protocolType}"); return;
            }
            
            Server.Handle();
        }

        public DamnClientConnection GetClientConnection(uint id) => Server.GetClientConnection(id);
        
        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(int clientConnectionId, ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new() => 
            await Server.SendAsync<TReceive>(clientConnectionId, sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(DamnClientConnection clientConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new() =>
            await Server.SendAsync<TReceive>(clientConnection, sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<DamnClientConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new() =>
            await Server.SendToSelectedAsync<TReceive>(predicate, sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType,
            params byte[] additionalData)
            where TReceive : ISerializable, new() =>
            await Server.SendToEachAsync<TReceive>(sendPacket, packetType, additionalData);

        public void Stop() => Server.Stop();
    }
}
#endif