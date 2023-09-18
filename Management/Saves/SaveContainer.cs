#if ENABLE_SERIALIZATION
using System.Reflection;
using System.Runtime.CompilerServices;
using DamnLibrary.Debugging;
using DamnLibrary.Serialization;

namespace DamnLibrary.Management.Saves
{
	public class SaveContainer : ISerializable
	{
		public MemberInfo MemberInfo { get; }
		public object Owner { get; }

		public SaveContainer(MemberInfo memberInfo, object owner)
		{
			MemberInfo = memberInfo;
			Owner = owner;
		}

		public void Serialize(SerializationStream stream)
		{
			var value = MemberInfo switch
			{
				FieldInfo fieldInfo => fieldInfo.GetValue(Owner),
				PropertyInfo propertyInfo => propertyInfo.GetValue(Owner),
				_ => null
			};

			stream.Write(MemberInfo.Name);
			stream.Write(value != null ? value.GetType() : MemberInfo.DeclaringType);
			var subStream = stream.CreateSerializationSubStream();
			if (value != null)
				subStream.Write(value, value.GetType());
			stream.Write(subStream.ToArray());
		}

		public void Deserialize(SerializationStream stream) =>
			UniversalDebugger.LogError($"[{nameof(SaveContainer)}] ({nameof(Deserialize)}) Unable to deserialize!");
	}
}
#endif