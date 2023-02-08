#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Networking.Packets;

namespace DamnLibrary.Networking.Protocols
{
    public interface IClientProtocol
    {
        public uint Id { get; set; }
        
        public bool IsConnected { get; }
        public bool IsPaused { get; set; }
        
        public Action OnConnect { get; set; }
        public Action<NetworkPacket> OnPacketReceive { get; set; }
        public Action OnDisconnect { get; set; }
        
        Task ConnectAsync(string address, int port);
        void Handle();
        Task WriteAsync(byte[] messageToSend);
        void Disconnect();
    }
}
#endif