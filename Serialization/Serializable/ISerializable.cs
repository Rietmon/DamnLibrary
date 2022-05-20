#if ENABLE_SERIALIZATION
namespace DamnLibrary.Serialization
{
    public interface ISerializable
    {
        void Serialize(SerializationStream stream);
        void Deserialize(SerializationStream stream);
    }
}
#endif