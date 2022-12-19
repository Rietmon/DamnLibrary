#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System.Net;
using System.Net.Sockets;
using DamnLibrary.Debugging;
using DamnLibrary.Networking.Client;
using DamnLibrary.Networking.Protocols;
using DamnLibrary.Networking.Protocols.TCP;

namespace DamnLibrary.Networking
{
    public class DamnServer
    {
        public bool IsWorking => Server.IsWorking;
        public bool IsPaused => Server.IsPaused;
        
        private IServerProtocol Server { get; set; }

        public void Start(DamnProtocolType damnProtocolType, string address, int port)
        {
            if (Server != null)
            {
                UniversalDebugger.LogError($"[{nameof(DamnServer)}] ({nameof(Start)}) Server is already started");
                return;
            }
            
            switch (damnProtocolType)
            {
                case DamnProtocolType.TCP: Server = new TCPServer(new TcpListener(IPAddress.Parse(address), port)); break;
                default: UniversalDebugger.LogError($"[{nameof(DamnServer)}] ({nameof(Start)}) Unknown protocol type, type = {damnProtocolType}"); return;
            }
            
            Server.Handle();
        }
    }
}
#endif