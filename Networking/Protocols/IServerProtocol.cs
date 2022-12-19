﻿#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
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
#endif