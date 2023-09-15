using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Identity;

public class NetworkIdentity : ISerializable
{
    public uint Connection { get; private set; }
    
    public uint Id { get; private set; }
    
    public void Serialize(SerializationStream stream)
    {
        stream.Write(Connection);
        stream.Write(Id);
    }

    public void Deserialize(SerializationStream stream)
    {
        Connection = stream.Read<uint>();
        Id = stream.Read<uint>();
    }
}