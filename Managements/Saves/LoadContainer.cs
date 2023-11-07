#if ENABLE_SERIALIZATION
namespace DamnLibrary.Managements.Saves
{
	public class LoadContainer : ISerializable
	{
		public string Name { get; private set; }
		
		public Type ValueType { get; private set; } 
		
		public object Value { get; private set; }
		
		public void Serialize(SerializationStream stream) =>
			UniversalDebugger.LogError($"[{nameof(LoadContainer)}] ({nameof(Serialize)}) Unable to serialize!");

		public void Deserialize(SerializationStream stream)
		{
			Name = stream.Read<string>();
			ValueType = stream.Read<Type>();
			var valueBytes = stream.Read<byte[]>();
			var subStream = stream.CreateDeserializationSubStream(valueBytes);
			Value = subStream.Read(ValueType);
		}
	}
}
#endif