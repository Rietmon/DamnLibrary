#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DamnLibrary.Networking.Handlers;

namespace DamnLibrary.Networking.Protocols.TCP
{
    public class TCPServer : ServerNetworkHandler, IServerProtocol
    {
        public override bool IsWorking { get; set; }
        
        public override bool IsPaused { get; set; }
        
        private TcpListener Server { get; set; }

        private List<DamnServerClientConnection> ClientConnections { get; } = new();

        public TCPServer(TcpListener server)
        {
            Server = server;
            
            Server.Start();
            IsWorking = true;
        }
        
        public void Stop()
        {
            IsWorking = false;
            Server.Stop();

            while (ClientConnections.Count > 0)
            {
                ClientConnections[0].Disconnect();
                ClientConnections.RemoveAt(0);
            }
        }

        protected override async Task OnHandleAsync()
        {
            var connection = await Server.AcceptTcpClientAsync();
            var clientConnection = new DamnServerClientConnection(new TCPClient(connection, false));
            ClientConnections.Add(clientConnection);
        }
    }
}
#endif