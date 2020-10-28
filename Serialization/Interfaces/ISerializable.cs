namespace Rietmon.Serialization
{
    public interface ISerializable
    {
        void Serialize(SerializationStream stream);
        void Deserialize(SerializationStream stream);
    }
}