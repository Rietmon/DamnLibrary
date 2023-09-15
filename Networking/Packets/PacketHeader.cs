using DamnLibrary.Networking.Identity;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets;

public class PacketHeader : ISerializable
{
    public ushort Type { get; private set; }
    public byte[] AdditionalData { get; private set; }

    public PacketHeader(ushort type, byte[] additionalData)
    {
        Type = type;
        AdditionalData = additionalData;
    }
    
    public void Serialize(SerializationStream stream)
    {
        stream.Write(Type);
        stream.Write(AdditionalData);
    }

    public void Deserialize(SerializationStream stream)
    {
        Type = stream.Read<ushort>();
        AdditionalData = stream.Read<byte[]>();
    }
}