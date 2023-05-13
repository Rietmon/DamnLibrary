﻿#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DamnLibrary.Debugging;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Server;
using DamnLibrary.Types;
using DamnLibrary.Serialization;
using UnityEngine;

namespace DamnLibrary.Networking.Protocols.TCP
{
    public class TCPServer : ServerNetworkHandler, IServerProtocol
    {
        public Func<Task> OnHandle { get; set; }
        public Action<ServerConnection> OnAcceptConnection { get; set; }
        public Action<ServerConnection> OnRejectingConnection { get; set; }
        public Action OnRejectConnection { get; set; }
        public Action OnUpdatedConnections { get; set; }
        
        public sealed override bool IsWorking { get; set; }
        public override bool IsPaused { get; set; }

        private TcpListener Server { get; set; }

        private List<ServerConnection> ServerConnections { get; } = new();

        public TCPServer(TcpListener server)
        {
            Server = server;

            Server.Start();
            Server.BeginAcceptTcpClient(AcceptConnection, Server);
            IsWorking = true;
        }

        public int GetServerConnectionsCount() => ServerConnections.Count;

        public ServerConnection GetServerConnection(uint id) =>
            ServerConnections.FirstOrDefault((connection) => connection.Id == id);

        public async Task<Pair<PacketHeader, TReceive>> SendAsync<TReceive>(ServerConnection serverConnection,
            ISerializable sendPacket,
            IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
            => await SendAsyncWithTimeout<TReceive>(serverConnection, sendPacket, packetType, additionalData);

        public async Task<Pair<PacketHeader, TReceive>[]> SendToSelectedAsync<TReceive>(
            Func<ServerConnection, bool> predicate,
            ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var connectionsToSend = ServerConnections.Where(predicate).ToArray();
            var responses = new Pair<PacketHeader, TReceive>[connectionsToSend.Length];
            
            for (var i = 0; i < connectionsToSend.Length; i++)
            {
                responses[i] = await SendAsyncWithTimeout<TReceive>(connectionsToSend[i], sendPacket, packetType, additionalData);
            }

            return responses;
        }

        public async Task<Pair<PacketHeader, TReceive>[]> SendToEachAsync<TReceive>(ISerializable sendPacket,
            IConvertible packetType, 
            params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var responses = new Pair<PacketHeader, TReceive>[ServerConnections.Count];
            for (var i = 0; i < ServerConnections.Count; i++)
            {
                responses[i] = await SendAsyncWithTimeout<TReceive>(ServerConnections[i], sendPacket, packetType, additionalData);
            }

            return responses;
        }

        public async Task SendWithoutResponseAsync(ServerConnection clientConnection, ISerializable sendPacket, IConvertible packetType,
            params byte[] additionalData) =>
            await clientConnection.SendWithoutResponseAsync(sendPacket, packetType, additionalData);
        
        public async Task SendToSelectedWithoutResponseAsync(Func<ServerConnection, bool> predicate, ISerializable sendPacket, IConvertible packetType,
            params byte[] additionalData)
        {
            var connectionsToSend = ServerConnections.Where(predicate).ToArray();
            
            for (var i = 0; i < connectionsToSend.Length; i++)
            {
                await connectionsToSend[i].SendWithoutResponseAsync(sendPacket, packetType, additionalData);
            }
        }

        public async Task SendToEachWithoutResponseAsync(ISerializable sendPacket, IConvertible packetType, params byte[] additionalData)
        {
            for (var i = 0; i < ServerConnections.Count; i++)
            {
                await ServerConnections[i].SendWithoutResponseAsync(sendPacket, packetType, additionalData);
            }
        }

        private static async Task<Pair<PacketHeader, TReceive>> SendAsyncWithTimeout<TReceive>(ServerConnection serverConnection,
            ISerializable sendPacket,
            IConvertible packetType, 
            params byte[] additionalData)
            where TReceive : ISerializable, new()
        {
            var sendTask = serverConnection.SendAsync<TReceive>(sendPacket, packetType, additionalData);
            var timeoutTask = Task.Delay(DamnNetworking.TimeoutForResponse);

            await Task.WhenAny(sendTask, timeoutTask);
            
            if (sendTask.IsCompleted)
                return sendTask.Result;

            UniversalDebugger.LogWarning(
                $"[{nameof(TCPServer)}] ({nameof(SendAsyncWithTimeout)}) Unable to get response from client. Id: {serverConnection.Id}");
            return default;
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            if (!IsWorking)
                return;
            
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(AcceptConnection)}) Accepting new connection...");
            var connection = Server.EndAcceptTcpClient(ar);
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(AcceptConnection)}) Found new client...");
            
            var serverConnection = new ServerConnection(new TCPClient(connection));
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(AcceptConnection)}) Accepted new connection! Id {serverConnection.Id}");
            
            serverConnection.OnDisconnect += () => OnRejectedConnection(serverConnection);
            ServerConnections.Add(serverConnection);
            OnAcceptConnection?.Invoke(serverConnection);

            Server.BeginAcceptTcpClient(AcceptConnection, Server);
            
            OnUpdatedConnections?.Invoke();
        }

        public void RejectConnection(ServerConnection serverConnection)
        {
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(RejectConnection)}) Rejecting connection with id {serverConnection.Id}...");
            OnRejectingConnection?.Invoke(serverConnection);
            
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(RejectConnection)}) Disconnecting id {serverConnection.Id}...");
            serverConnection.Disconnect();
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(RejectConnection)}) Id {serverConnection.Id} disconnected");
            
            ServerConnections.Remove(serverConnection);
                
            OnRejectConnection?.Invoke();
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(RejectConnection)}) Connection with {serverConnection.Id} rejected");
        }

        public void Stop()
        {
            IsWorking = false;
            Server.Stop();

            while (ServerConnections.Count > 0)
            {
                RejectConnection(ServerConnections[0]);
            }
        }

        protected override async Task OnHandleAsync()
        {
            for (var i = 0; i < ServerConnections.Count; i++)
            {
                var serverConnection = ServerConnections[i];
                if (!serverConnection.IsConnected)
                {
                    OnRejectedConnection(serverConnection);
                    i--;
                }
            }

            if (OnHandle != null)
                await OnHandle.Invoke();
        }

        private void OnRejectedConnection(ServerConnection serverConnection)
        {
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(OnRejectedConnection)}) Found rejected connection with id {serverConnection.Id}, removing connection...");
            OnRejectingConnection?.Invoke(serverConnection);
                
            ServerConnections.Remove(serverConnection);
                
            OnRejectConnection?.Invoke();
            UniversalDebugger.Log($"[{nameof(TCPServer)}] ({nameof(OnRejectedConnection)}) Connection with id {serverConnection.Id} has been removed");
        }
    }
}
#endif