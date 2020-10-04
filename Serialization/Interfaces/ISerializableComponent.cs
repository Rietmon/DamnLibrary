namespace Rietmon.Common.Serialization
{
    public interface ISerializableComponent
    {
        void Serialize(SerializationStream stream);
        void Deserialize(SerializationStream stream);
    }
}
