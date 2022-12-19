#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Networking.Packets;

namespace DamnLibrary.Networking.Protocols
{
    public interface IClientProtocol
    {
        public bool IsConnected { get; }
        public bool IsPaused { get; set; }
        public bool IsAvailable { get; }
        
        public Action<NetworkPacket> OnPacketReceived { get; set; }

        void Handle();

        Task WriteAsync(byte[] messageToSend, CancellationToken cancellationToken);

        void Disconnect();
    }
}
#endif