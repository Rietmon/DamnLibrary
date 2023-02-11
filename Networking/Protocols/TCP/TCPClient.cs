#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Debugging;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Serialization;
using UnityEngine;
using UnityEngine.Profiling;

namespace DamnLibrary.Networking.Protocols.TCP
{
    public class TCPClient : ClientNetworkHandler, IClientProtocol
    {
        public uint ConnectionId { get; set; }
        
        public Action OnConnect { get; set; }
        public Action<NetworkPacket> OnPacketReceive { get; set; }
        public Action OnDisconnect { get; set; }

        public override bool IsConnected => Client.Connected;
        public override bool IsPaused { get; set; }

        private TcpClient Client { get; set; }
        
        private NetworkStream Stream { get; set; }

        private byte[] Buffer { get; } = new byte[DamnNetworking.MaxPacketLength];

        internal TCPClient(TcpClient client)
        {
            Client = client;

            if (!client.Connected) 
                return;
            
            OnConnect?.Invoke();
            Stream = Client.GetStream();
        }

        public async Task ConnectAsync(string address, int port)
        {
            try
            {
                if (!Client.Connected)
                {
                    await Client.ConnectAsync(address, port);
                }

                OnConnect?.Invoke();
                Stream = Client.GetStream();
            }
            catch (SocketException socketException)
            {
                switch (socketException.SocketErrorCode)
                {
                    case SocketError.ConnectionRefused:
                    {
                        UniversalDebugger.LogError($"[{nameof(TCPClient)}] ({nameof(ConnectAsync)}) Unable connect to {address}:{port}. Target machine is probably not active");
                        break;
                    }
                }
            }
        }

        public async Task WriteAsync(byte[] messageToSend)
        {
            var sendTask = Stream.WriteAsync(messageToSend, 0, messageToSend.Length);
            var timeoutTask = Task.Delay(DamnNetworking.TimeoutForSend);
            await Task.WhenAny(sendTask, timeoutTask);
            
            if (!sendTask.IsCompleted)
                UniversalDebugger.LogError($"[{nameof(TCPClient)}] ({nameof(WriteAsync)}) Reached timeout while sending message to id {ConnectionId}. Message might be not delivered!");
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
            
            OnDisconnect?.Invoke();
            Client.Dispose();
            UniversalDebugger.Log($"[{nameof(TCPClient)}] ({nameof(Disconnect)}) Disconnected. Id: {ConnectionId}");
        }

        protected override async Task OnHandleAsync()
        {
            try
            {
                Profiler.BeginSample("Read server");
                var bytesRead = await Stream.ReadAsync(Buffer, 0, 2);
                if (!ValidateReadPacket(bytesRead))
                    return;

                var deserializationStream = new SerializationStream(Buffer);

                var packetSize = deserializationStream.Read<ushort>();
                if (packetSize > DamnNetworking.MaxPacketLength)
                {
                    UniversalDebugger.LogError(
                        $"[{nameof(TCPClient)}] ({nameof(OnHandleAsync)}) packetSize is larger than MaxPacketLength: {packetSize} > {DamnNetworking.MaxPacketLength}. Disconnecting...");
                    Disconnect();
                    return;
                }

                bytesRead = await Stream.ReadAsync(Buffer, 0, packetSize);
                if (!ValidateReadPacket(bytesRead))
                    return;

                deserializationStream.Position = 0;

                var packetHeader = deserializationStream.Read<PacketHeader>();
                var networkPacket = new NetworkPacket(packetHeader, deserializationStream);

                OnPacketReceive?.Invoke(networkPacket);
                Profiler.EndSample();

                if (!packetHeader.IsResponse && !networkPacket.IsHandled)
                    UniversalDebugger.LogError($"[{nameof(TCPClient)}] ({nameof(OnHandleAsync)}) NetworkPacket didnt handled. Id = {networkPacket.Header.Id}, Type = {networkPacket.Header.Type}");
            }
            catch (IOException ioException)
            {
                if (ioException.InnerException is SocketException socketException)
                {
                    switch (socketException.SocketErrorCode)
                    {
                        case SocketError.OperationAborted:
                        {
                            UniversalDebugger.LogError($"[{nameof(TCPClient)}] ({nameof(OnHandleAsync)}) Operation aborted. Probably connection was closed. Disconnecting...");
                            Disconnect();
                            break;
                        }
                    }
                }
            }
        }

        private bool ValidateReadPacket(int bytesRead)
        {
            if (bytesRead > 0)
                return true;
            
            UniversalDebugger.LogError($"[{nameof(TCPClient)}] ({nameof(ValidateReadPacket)}) Read empty response. Id: {ConnectionId}. Disconnecting...");
            Disconnect();
            return false;
        }
    }
}
#endif