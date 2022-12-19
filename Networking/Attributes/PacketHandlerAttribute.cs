#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
namespace DamnLibrary.Networking.Attributes
{
    public sealed class PacketHandlerAttribute : Attribute
    {
        public IConvertible Type { get; }

        public PacketHandlerAttribute(byte type) => Type = type;
        public PacketHandlerAttribute(sbyte type) => Type = type;
        public PacketHandlerAttribute(short type) => Type = type;
        public PacketHandlerAttribute(ushort type) => Type = type;
        public PacketHandlerAttribute(int type) => Type = type;
        public PacketHandlerAttribute(uint type) => Type = type;
        public PacketHandlerAttribute(long type) => Type = type;
        public PacketHandlerAttribute(ulong type) => Type = type;
    }
}
#endif