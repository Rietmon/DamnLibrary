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
    
        [SerializeReference] private UnityBehaviour[] serializableComponents;

        private static readonly List<ISerializable> serializables = new List<ISerializable>();

        private void Awake()
        {
            Instance = this;
        }

        public static void AddToSerializables(ISerializable serializable) => serializables.Add(serializable);

        public static void RemoveFromSerializables(ISerializable serializable) => serializables.Remove(serializable);
    
        public static byte[] Serialize()
        {
            var stream = new SerializationStream();

            stream.Write(Version);

            var components = Instance.serializableComponents;

            foreach (var component in components)
                ((ISerializableComponent)component).Serialize(stream);

            foreach (var serializable in serializables)
                serializable.Serialize(stream);

            return stream.ToArray();
        }
    
        public static void Deserialize(byte[] bytes)
        {
            var stream = new SerializationStream(bytes);

            var version = stream.Read<short>();
            stream.Version = version;

            var components = Instance.serializableComponents;
        
            foreach (var component in components)
                ((ISerializableComponent)component).Deserialize(stream);
            
            foreach (var serializable in serializables)
                serializable.Deserialize(stream);
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
