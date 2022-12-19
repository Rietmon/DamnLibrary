using System;
using System.Threading;
using DamnLibrary.Networking.Packets;

namespace DamnLibrary.Networking.Protocols
{
    public interface IServerProtocol
    {
        public bool IsWorking { get; }
        
        public bool IsPaused { get; }

        void Handle();

        void Stop();
    }
}