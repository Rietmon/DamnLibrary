using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Rietmon.Behaviours;
using Rietmon.DS;
using Rietmon.Extensions;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class Serialization : UnityBehaviour
    {
        public const short Version = 1;
        
        public static Serialization Instance { get; private set; }

        private static List<Type> serializableStaticTypes;
    
        [SerializeField] private SerializableObject[] serializableObjects;

        private void OnEnable()
        {
            Instance = this;
        }

        public static async UniTask InitializeAsync()
        {
            serializableStaticTypes =
                new List<Type>(AssemblyUtilities.GetAllAttributeInherits<StaticSerializableAttribute>());
        }

        public static SerializationStream CreateSerializationStream()
        {
            var stream = new SerializationStream {Version = Version};
            stream.Write(Version);
            return stream;
        }

        public static SerializationStream CreateDeserializationStream(byte[] bytes)
        {
            var stream = new SerializationStream(bytes);
            stream.Version = stream.Read<short>();
            return stream;
        }

        public static bool IsWillSerializable(SerializableObject obj) => Instance.serializableObjects.Contains(obj);

        public static byte[] SerializeComponents(SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize, because there is no serialization component!");
                return new byte[0];
            }
            stream ??= CreateSerializationStream();

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize, because there is no serializable objects!");
                return new byte[0];
            }

            foreach (var obj in objects)
                ForeachSerializableObjectComponent(obj, (component) => component.Serialize(stream));
            
            return stream.ToArray();
        }

        public static void DeserializeComponents(byte[] bytes, SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize, because there is no serialization component!");
                return;
            }

            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize, because byte is null or length is equal 0!");
                return;
            }
            
            stream ??= CreateDeserializationStream(bytes);

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize, because there is no serializable objects!");
                return;
            }

            foreach (var obj in objects)
                ForeachSerializableObjectComponent(obj, (component) => component.Deserialize(stream));
        }

        public static byte[] SerializeStaticTypes(SerializationStream stream = null)
        {
            stream ??= CreateSerializationStream();
            
            foreach (var type in serializableStaticTypes)
                type.SafeInvokeStaticMethod("Serialize", stream);

            return stream.ToArray();
        }

        public static void DeserializeStaticTypes(byte[] bytes, SerializationStream stream = null)
        {
            stream ??= CreateDeserializationStream(bytes);
            
            foreach (var type in serializableStaticTypes)
                type.SafeInvokeStaticMethod("Deserialize", stream);
        }

        private static void ForeachSerializableObjectComponent(SerializableObject serializableObject,
            Action<ISerializable> callback)
        {
            foreach (var component in serializableObject.SerializableComponents)
            {
                callback?.Invoke(component);
            }
        }

        // EDITOR METHODS
        
#if UNITY_EDITOR

        [ContextMenu("Find serializable components")]
        private void FindSerializableComponents()
        {
            serializableObjects = FindObjectsOfType<SerializableObject>();
        
            Debug.Log($"Founded {serializableObjects.Length} components");
        }

#endif
    }
}
