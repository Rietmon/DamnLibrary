#if ENABLE_SERIALIZATION
namespace Rietmon.Serialization
{
    public interface ISerializable
    {
        void Serialize(SerializationStream stream);
        void Deserialize(SerializationStream stream);
    }
}
#endif