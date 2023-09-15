using DamnLibrary.Debugging;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets.Handles;

public static class PacketHandler
{
    private static Dictionary<ushort, Action<PacketHeader, SerializationStream>> Handlers { get; } = new();

    public static void RegisterHandler(IConvertible type, Action<PacketHeader, SerializationStream> method) =>
        Handlers.Add(type.ToUInt16(null), method);

    internal static void HandlePacket(PacketHeader header, SerializationStream stream)
    {
        if (!Handlers.TryGetValue(header.Type, out var handler))
        {
            UniversalDebugger.LogWarning($"[{nameof(PacketHandler)}] ({nameof(HandlePacket)}) Unable to handle packet with type={header.Type}");
            return;
        }

        handler(header, stream);
    }
}