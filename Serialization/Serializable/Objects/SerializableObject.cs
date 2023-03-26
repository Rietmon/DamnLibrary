﻿#if ENABLE_SERIALIZATION && UNITY_5_3_OR_NEWER 
using System.Collections.Generic;
using DamnLibrary.Debugging;
using UnityEngine;

namespace DamnLibrary.Serialization
{
    [AddComponentMenu("Serialization/SerializableObject")]
    public class SerializableObject : SerializableDamnBehaviour
    {
        public List<SerializableDamnBehaviour> SerializableComponents { get; private set; }
        
        [SerializeField] private bool serializeAllComponents;

        [SerializeField] private SerializableDamnBehaviour[] serializableComponents;

        private void OnEnable()
        {
            if (SerializableComponents != null)
                return;
            
            SerializableComponents = new List<SerializableDamnBehaviour>(serializeAllComponents 
                ? GetComponents<SerializableDamnBehaviour>() 
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
                    UniversalDebugger.LogError($"[{nameof(SerializableObject)}] ({nameof(OnDeserialize)}) Unable to find component with id {id}");
                    continue;
                }
                component.Deserialize(stream.CreateDeserializationSubStream(bytes));
            }
        }

        protected override void Reset()
        {
            base.Reset();
            serializeAllComponents = true;
        }
    }
}
#endif