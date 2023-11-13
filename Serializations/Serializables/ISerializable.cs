#if ENABLE_SERIALIZATION
namespace DamnLibrary.Serializations.Serializables
{
    public interface ISerializable
    {
        void Serialize(SerializationStream stream);
        void Deserialize(SerializationStream stream);
    }
}
#endif