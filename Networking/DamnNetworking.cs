#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
namespace DamnLibrary.Networking
{
    public static class DamnNetworking
    {
        public const string LocalHost = "127.0.0.1";
        
        public static int HandleDelay { get; set; } = 10;

        public static ushort MaxPacketLength { get; set; } = 256;
    }
}
#endif