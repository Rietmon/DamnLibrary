#if ENABLE_SERIALIZATION && UNITY_2020
using System.Collections.Generic;
using UnityEngine;

namespace Rietmon.Serialization
{
    [AddComponentMenu("Serialization/SerializableObject")]
    public class SerializableObject : SerializableUnityBehaviour
    {
        public List<SerializableUnityBehaviour> SerializableComponents { get; private set; }
        
        [SerializeField] private bool serializeAllComponents;

        [SerializeField] private SerializableUnityBehaviour[] serializableComponents;

        private void OnEnable()
        {
            if (SerializableComponents != null)
                return;
            
            SerializableComponents = new List<SerializableUnityBehaviour>(serializeAllComponents 
                ? GetComponents<SerializableUnityBehaviour>() 
                : serializableComponents);
            SerializableComponents.Remove(this);
        }

        protected override void OnSerialize(SerializationStream stream)
        {
            foreach (var component in SerializableComponents)
            {
                var subStream = stream.CreateSerializationSubStream();
                component.Serialize(subStream);
                
                if (subStream.IsEmpty)
                    continue;
                
                stream.Write(component.SerializableId);
                stream.Write(subStream.ToArray());
            }
        }

        protected override void OnDeserialize(SerializationStream stream)
        {
            while (stream.HasBytesToRead)
            {
                var id = stream.Read<short>();
                var bytes = stream.Read<byte[]>();
                var component = SerializableComponents.Find((c) => c.SerializableId == id);
                if (!component)
                {
                    Debug.LogError($"[{nameof(SerializableObject)}] ({nameof(OnDeserialize)}) Unable to find component with id {id}");
                    continue;
                }
                component.Deserialize(stream.CreateDeserializationSubStream(bytes));
            }
        }

        private void Reset()
        {
            serializeAllComponents = true;
        }
    }
}
#endif