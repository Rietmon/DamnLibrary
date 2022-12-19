using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Networking.Protocols.TCP;
using DamnLibrary.Other;
using DamnLibrary.Serialization;
using ProtocolType = DamnLibrary.Networking.Protocols.ProtocolType;

namespace DamnLibrary.Networking.Client
{
    public class DamnClient
    {
        public bool IsConnected => Client.IsConnected;
        public bool IsAvailable => Client.IsAvailable;

        public bool IsPaused { get => Client.IsPaused; set => Client.IsPaused = value; }

        private uint LastSentPacketId { get; set; }
        private IClientProtocol Client { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; } = new();

        internal DamnClient(IClientProtocol client)
        {
            Client = client;
            Client.OnPacketReceived += OnPacketReceived;
            Client.Handle();
        }

        public async Task Connect(ProtocolType protocolType, string address, int port)
        {
            if (Client != null)
            {
                UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(Connect)}) Client is already connected");
                return;
            }
            
            switch (protocolType)
            {
                case ProtocolType.TCP: Client = new TCPClient(new TcpClient(address, port)); break;
                default: UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(Connect)}) Unknown protocol type, type = {protocolType}"); return;
            }

            await TaskUtilities.WaitUntil(() => IsAvailable);

            Client.OnPacketReceived += OnPacketReceived;
            Client.Handle();
        }

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(ISerializable sendPacket, IConvertible paketType, params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var sendPacketHeader = new PacketHeader
            {
                Id = LastSentPacketId++,
                IsResponse = false,
                Type = paketType,
                AdditionData = additionalData
            };

            var messageToSend = CreateMessage(sendPacketHeader, sendPacket);
            
            UniversalDebugger.Log($"[{nameof(DamnClient)}] ({nameof(SendAsync)}) Sending {paketType}, size = {messageToSend.Length}b");

            await Client.WriteAsync(messageToSend, CancellationTokenSource.Token);

            var responseNetworkPacket = await WaitForNetworkPacket(sendPacketHeader.Id);
            
            var deserializationStream = responseNetworkPacket.DeserializationStream;
            var receivedPacketHeader = responseNetworkPacket.Header;
            var receivedPacket = new TReceive();
            receivedPacket.Deserialize(deserializationStream);
            responseNetworkPacket.Dispose();
        
            return new Pair<PacketHeader, TReceive>(receivedPacketHeader, receivedPacket);
        }

        public async Task SendResponseAsync(PacketHeader receivedPacketHeader, ISerializable sendPacket)
        {
            receivedPacketHeader.IsResponse = true;
            
            var messageToSend = CreateMessage(receivedPacketHeader, sendPacket);
            
            UniversalDebugger.Log($"[{nameof(DamnClient)}] ({nameof(SendResponseAsync)}) Sending {receivedPacketHeader.Type}, size = {messageToSend.Length}b");

            await Client.WriteAsync(messageToSend, CancellationTokenSource.Token);
        }

        public void Disconnect()
        {
            CancellationTokenSource.Cancel();
            Client.Disconnect();
        }

        private byte[] CreateMessage(PacketHeader packetHeader, ISerializable packet)
        {
            var serializationStream = new SerializationStream();
            serializationStream.WriteFromAndReturn(2, packetHeader);
            serializationStream.WriteFromAndReturn(serializationStream.Length, packet);
            serializationStream.WriteFromAndReturn(0, (ushort)(serializationStream.Length - 2));

            var messageToSend = serializationStream.ToArray();
            serializationStream.Dispose();
            
            return messageToSend;
        }
        
        private async Task<NetworkPacket> WaitForNetworkPacket(uint packetId)
        {
            NetworkPacket responsePacket = null;
            void OnNetworkPacketReceived(NetworkPacket networkPacket)
            {
                if (networkPacket.Header.Id != packetId) 
                    return;
                
                responsePacket = networkPacket;
                responsePacket.IsHandled = true;
            }
            
            Client.OnPacketReceived += OnNetworkPacketReceived;
            await TaskUtilities.WaitUntil(() => responsePacket != null);
            Client.OnPacketReceived -= OnNetworkPacketReceived;
            
            return responsePacket;
        }

        private void OnPacketReceived(NetworkPacket networkPacket)
        {
            if (networkPacket.Header.IsResponse) 
                return;
            
            var sendPacket = PacketHandler.Handle(networkPacket.Header.Type, networkPacket.DeserializationStream);
            SendResponseAsync(networkPacket.Header, sendPacket).Forget();
        }
    }
}