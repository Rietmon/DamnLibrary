#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
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
    public sealed class DamnClient
    {
        public uint ConnectionId { get => Client.ConnectionId; set => Client.ConnectionId = value; }
        
        public long UserId { get; set; }
        
        public DateTime LastPacketHandle { get; internal set; }
        
        public Action OnConnect { get => Client.OnConnect; set => Client.OnConnect = value; }
        
        public Action<NetworkPacket> OnPacketReceive { get => Client.OnPacketReceive; set => Client.OnPacketReceive = value; }

        public Action OnDisconnect { get => Client.OnDisconnect; set => Client.OnDisconnect = value; }
        
        public bool IsConnected => Client.IsConnected;
        public bool IsPaused { get => Client.IsPaused; set => Client.IsPaused = value; }
        public bool IsServerConnection { get; }
        private uint LastSentPacketId { get; set; }
        private IClientProtocol Client { get; set; }

        public DamnClient()
        {
            LastSentPacketId = uint.MaxValue;
            IsServerConnection = true;
        }
        
        internal DamnClient(IClientProtocol client, uint connectionId)
        {
            Client = client;
            ConnectionId = connectionId;
            OnPacketReceive += OnPacketReceived;
            Client.Handle();
        }

        public async Task ConnectAsync(ProtocolType protocolType, string address, int port)
        {
            if (Client != null && Client.IsConnected)
            {
                UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(ConnectAsync)}) Client is already connected");
                return;
            }
            
            switch (protocolType)
            {
                case ProtocolType.TCP: Client = new TCPClient(new TcpClient()); break;
                default: UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(ConnectAsync)}) Unknown protocol type, type = {protocolType}"); return;
            }

            await Client.ConnectAsync(address, port);

            if (!Client.IsConnected)
                return;

            Client.OnPacketReceive += OnPacketReceived;
            Client.Handle();
        }
        
        public async Task SendAsyncWithoutResponse(ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
        {
            if (!IsConnected)
            {
                UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(SendAsync)}) Trying to send from a non-connected client");
                return;
            }
            
            var sendPacketHeader = new PacketHeader
            {
                Id = LastSentPacketId++,
                IsResponse = false,
                NeedResponse = false,
                Type = packetType,
                AdditionData = additionalData
            };

            var messageToSend = CreateMessage(sendPacketHeader, sendPacket);
            
            //UniversalDebugger.Log($"[{nameof(DamnClient)}] ({nameof(SendAsync)}) Sending {packetType}, size = {messageToSend.Length}b");

            await Client.WriteAsync(messageToSend);
        }

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            if (!IsConnected)
            {
                UniversalDebugger.LogError($"[{nameof(DamnClient)}] ({nameof(SendAsync)}) Trying to send from a non-connected client");
                return default;
            }
            
            var sendPacketHeader = new PacketHeader
            {
                Id = GetNextPacketId(),
                IsResponse = false,
                NeedResponse = true,
                Type = packetType,
                AdditionData = additionalData
            };

            var messageToSend = CreateMessage(sendPacketHeader, sendPacket);
            
            //UniversalDebugger.Log($"[{nameof(DamnClient)}] ({nameof(SendAsync)}) Sending {packetType}, size = {messageToSend.Length}b");

            await Client.WriteAsync(messageToSend);

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
            
            //UniversalDebugger.Log($"[{nameof(DamnClient)}] ({nameof(SendResponseAsync)}) Sending {receivedPacketHeader.Type}, size = {messageToSend.Length}b");

            await Client.WriteAsync(messageToSend);
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
            
            Client.Disconnect();
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
            
            Client.OnPacketReceive += OnNetworkPacketReceived;
            await TaskUtilities.WaitUntil(() => responsePacket != null);
            Client.OnPacketReceive -= OnNetworkPacketReceived;
            
            return responsePacket;
        }

        private void OnPacketReceived(NetworkPacket networkPacket)
        {
            if (networkPacket.Header.IsResponse) 
                return;
            
            var sendPacket = PacketHandler.Handle(this, networkPacket.Header, networkPacket.DeserializationStream);
            
            if (networkPacket.Header.NeedResponse)
                SendResponseAsync(networkPacket.Header, sendPacket).Forget();
            
            networkPacket.IsHandled = true;
        }

        private uint GetNextPacketId() => IsServerConnection ? --LastSentPacketId : ++LastSentPacketId;

        private static byte[] CreateMessage(PacketHeader packetHeader, ISerializable packet)
        {
            var serializationStream = new SerializationStream();
            serializationStream.WriteFromAndReturn(2, packetHeader);
            serializationStream.WriteFromAndReturn(serializationStream.Length, packet);
            serializationStream.WriteFromAndReturn(0, (ushort)(serializationStream.Length - 2));

            var messageToSend = serializationStream.ToArray();
            serializationStream.Dispose();
            
            return messageToSend;
        }
    }
}
#endif