#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using DamnLibrary.Networking.Handlers;
using DamnLibrary.Networking.Packets;

namespace DamnLibrary.Networking
{
    public static class DamnNetworking
    {
        public static int ServerHandleDelay { get; set; } = 50;
        public static int ClientHandleDelay { get; set; } = 15;

        public static ushort MaxPacketLength { get; set; } = 256;

        public static int TimeoutForSend { get; set; } = 50;
        public static int TimeoutForResponse { get; set; } = 1000;
    }
}
#endif