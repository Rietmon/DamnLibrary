using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Extensions;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Serialization;
using UnityEngine;

namespace DamnLibrary.Networking.Protocols.TCP
{
    public class TCPClient : ClientNetworkHandler, IClientProtocol
    {
        public Action<NetworkPacket> OnPacketReceived { get; set; }

        public override bool IsConnected => Client.Connected;

        public override bool IsPaused { get; set; }

        private TcpClient Client { get; }
        
        private NetworkStream Stream { get; set; }
        
        private byte[] Buffer { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; } = new();
        
        public TCPClient(TcpClient tcpClient, bool waitForConnect = true)
        {
            Client = tcpClient;

            if (!waitForConnect)
                return;
            
            async void WaitForConnect()
            {
                await TaskUtilities.WaitUntil(() => Client.Connected);
                IsAvailable = true;
                Stream = Client.GetStream();
            }
            
            WaitForConnect();
        }
        
        public async Task WriteAsync(byte[] messageToSend, CancellationToken cancellationToken)
        {
            await Stream.WriteAsync(messageToSend, 0, messageToSend.Length, cancellationToken);
        }

        public void Disconnect()
        {
            CancellationTokenSource.Cancel();
            Client.Close();
            Stream.Dispose();
        }

        protected override async Task OnHandleAsync()
        {
            var bytesRead = await Stream.ReadAsync(Buffer, 0, 2, CancellationTokenSource.Token);
            if (!ValidateReadPacket(bytesRead))
                return;

            var deserializationStream = new SerializationStream(Buffer);
            
            var packetSize = deserializationStream.Read<ushort>();
            if (packetSize > DamnNetworking.MaxPacketLength)
            {
                Debug.LogError($"[{nameof(TCPClient)}] ({nameof(OnHandleAsync)}) packetSize is larger than MaxPacketLength. {packetSize} > {DamnNetworking.MaxPacketLength}");
                Disconnect();
                return;
            }
            
            bytesRead = await Stream.ReadAsync(Buffer, 0, packetSize, CancellationTokenSource.Token);
            if (!ValidateReadPacket(bytesRead))
                return;

            deserializationStream.Position = 0;
            
            var inPacketHeader = deserializationStream.Read<PacketHeader>();
            var networkPacket = new NetworkPacket(inPacketHeader, deserializationStream);

            OnPacketReceived.Invoke(networkPacket);

            if (!networkPacket.IsHandled)
                Debug.LogError($"[{nameof(TCPClient)}] ({nameof(OnHandleAsync)}) NetworkPacket didnt handled. Id = {networkPacket.Header.Id}, Type = {networkPacket.Header.Type}");

        }
    }
}