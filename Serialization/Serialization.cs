using System.Collections.Generic;
using System.Linq;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class Serialization : UnityBehaviour
    {
        public const byte Version = 1;
        
        public static Serialization Instance { get; private set; }
    
        [SerializeReference] private UnityBehaviour[] serializableComponents;

        private void Awake()
        {
            Instance = this;
        }
    
        public static byte[] Serialize()
        {
            var stream = new SerializationStream();

            stream.Write(Version);

            var components = Instance.serializableComponents;

            foreach (var component in components)
            {
                ((ISerializableComponent)component).Serialize(stream);
            }

            return stream.ToArray();
        }
    
        public static void Deserialize(byte[] bytes)
        {
            var stream = new SerializationStream(bytes);

            var version = stream.Read<byte>();
            stream.Version = version;

            var components = Instance.serializableComponents;
        
            foreach (var component in components)
            {
                ((ISerializableComponent)component).Deserialize(stream);
            }
        }

        // EDITOR METHODS
        
#if UNITY_EDITOR

        [ContextMenu("Find serializable components")]
        public void Editor_FindSerializableComponents()
        {
            var serializeObjects = FindObjectsOfType<SerializableObject>();

            var result = new List<ISerializableComponent>();

            var componentsFounded = 0;
            foreach (var obj in serializeObjects)
            {
                var components = obj.GetComponents<ISerializableComponent>(); 
                result.AddRange(components);
                componentsFounded += components.Length;
            }

            serializableComponents = result.Cast<UnityBehaviour>().ToArray();
        
            Debug.Log($"Founded {componentsFounded} components");
        }

#endif
    }
}
