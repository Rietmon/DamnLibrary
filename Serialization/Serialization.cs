using System;
using System.Collections.Generic;
using System.Linq;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class Serialization : UnityBehaviour
    {
        public const short Version = 1;
        
        public static Serialization Instance { get; private set; }
    
        [SerializeField] private UnityBehaviour[] serializableComponents;

        private void Awake()
        {
            Instance = this;
        }

        public static SerializationStream CreateSerializationStream()
        {
            var stream = new SerializationStream();
            stream.Write(Version);
            return stream;
        }

        public static SerializationStream CreateDeserializationStream(byte[] bytes)
        {
            var stream = new SerializationStream(bytes);
            stream.Version = stream.Read<short>();
            return stream;
        }
    
        public static byte[] SerializeAll(SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeAll)}) Unable to serialize, because there is no serialization component!");
                return new byte[0];
            }
            stream ??= CreateSerializationStream();

            var components = Instance.serializableComponents;

            foreach (var component in components)
                ((ISerializable)component).Serialize(stream);

            return stream.ToArray();
        }

        public static void DeserializeAll(byte[] bytes, SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeAll)}) Unable to deserialize, because there is no serialization component!");
                return;
            }
            
            stream ??= CreateDeserializationStream(bytes);

            var components = Instance.serializableComponents;
        
            foreach (var component in components)
                ((ISerializable)component).Deserialize(stream);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        // EDITOR METHODS
        
#if UNITY_EDITOR

        [ContextMenu("Find serializable components")]
        private void FindSerializableComponents()
        {
            var serializeObjects = FindObjectsOfType<SerializableObject>();

            var result = new List<ISerializable>();

            var componentsFounded = 0;
            foreach (var obj in serializeObjects)
            {
                var components = obj.GetComponents<ISerializable>(); 
                result.AddRange(components);
                componentsFounded += components.Length;
            }

            serializableComponents = result.Cast<UnityBehaviour>().ToArray();
        
            Debug.Log($"Founded {componentsFounded} components");
        }

#endif
    }
}
