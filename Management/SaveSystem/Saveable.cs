using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DamnLibrary.Extensions;
using DamnLibrary.Serialization;

namespace DamnLibrary.Management
{
	public abstract class Saveable : ISerializable
	{
		private List<SaveContainer> saveContainers;
		
		public byte[] Save()
		{
			if (saveContainers == null)
				CreateSaveContainers();

			var serializationStream = new SerializationStream();
			serializationStream.Write(saveContainers);

			return serializationStream.ToArray();
		}

		public void Load(byte[] data)
		{
			if (saveContainers == null)
				CreateSaveContainers();
			
			var deserializationStream = new SerializationStream(data);
			var loadContainers = deserializationStream.Read<List<LoadContainer>>();

			foreach (var loadContainer in loadContainers)
			{
				var saveContainer = saveContainers!.FirstOrDefault((container) => container.MemberInfo.Name == loadContainer.Name);

				saveContainer?.MemberInfo.SetValue(loadContainer.Value, this);
			}
		}

		private void CreateSaveContainers()
		{
			var type = GetType();
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var properties =
				type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var fieldsToSave = fields
				.Where((field) => field.GetCustomAttribute<SaveAttribute>() != null);
			var propertiesToSave = properties
				.Where((property) => property.GetCustomAttribute<SaveAttribute>() != null);

			saveContainers = new List<SaveContainer>();
			foreach (var field in fieldsToSave)
				saveContainers.Add(new SaveContainer(field, this));
			foreach (var property in propertiesToSave)
				saveContainers.Add(new SaveContainer(property, this));
		}

		public void Serialize(SerializationStream stream)
		{
			stream.Write(Save());
		}

		public void Deserialize(SerializationStream stream)
		{
			var bytes = stream.Read<byte[]>();
			Load(bytes);
		}
		
		protected abstract Type GetTypeFromAssembly(string name);
	}
}