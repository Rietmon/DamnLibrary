#if ENABLE_SERIALIZATION
using System;
using System.Collections.Generic;
using Rietmon.Behaviours;
using Rietmon.Extensions;
using Rietmon.Management;
using UnityEditor;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class Serialization : UnityBehaviour
    {
        public const short Version = 1;
        
        public static Serialization Instance { get; private set; }

        private static readonly Dictionary<short, Type> serializableStaticTypes = new Dictionary<short, Type>();
    
        #if UNITY_2020
        [SerializeField] private SerializableObject[] serializableObjects;
        #endif

        private void OnEnable()
        {
            Instance = this;
        }

        public static void Initialize()
        {
            var inheritors = AssemblyUtilities.GetAllAttributeInherits<StaticSerializableAttribute>();
            foreach (var inheritor in inheritors)
            {
                var attribute = (StaticSerializableAttribute)Attribute.GetCustomAttribute(inheritor, 
                    typeof(StaticSerializableAttribute));
                serializableStaticTypes.Add(attribute.SerializableId, inheritor);
            }
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

#if UNITY_2020
        public static byte[] SerializeComponents(SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize at the {SceneManager.ActiveScene.name} scene, because there is no serialization component!");
                return new byte[0];
            }
            
            stream ??= CreateSerializationStream();

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize at the {SceneManager.ActiveScene.name} scene, because there is no serializable objects!");
                return new byte[0];
            }

            foreach (var obj in objects)
            {
                if (!obj)
                    continue;
                var subStream = stream.CreateSerializationSubStream();
                obj.Serialize(subStream);

                if (subStream.IsEmpty)
                    continue;
                
                stream.Write(obj.SerializableId);
                stream.Write(subStream.ToArray());
            }
            
            return stream.ToArray();
        }
#endif

#if UNITY_2020
        public static void DeserializeComponents(byte[] bytes, SerializationStream stream = null)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because there is no serialization component!");
                return;
            }

            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because byte is null or length is equal 0!");
                return;
            }
            
            stream ??= CreateDeserializationStream(bytes);

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                Debug.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because there is no serializable objects!");
                return;
            }

            while (stream.HasBytesToRead)
            {
                var id = stream.Read<short>();
                var componentBytes = stream.Read<byte[]>();
                var component = Instance.serializableObjects.Find((c) => c.SerializableId == id);
                if (!component)
                {
                    Debug.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to find component with id {id} at the {SceneManager.ActiveScene.name} scene");
                    continue;
                }

                component.Deserialize(stream.CreateDeserializationSubStream(componentBytes));
            }
        }
#endif
        
        public static byte[] SerializeStaticTypes(SerializationStream stream = null)
        {
            stream ??= CreateSerializationStream();
            
            foreach (var type in serializableStaticTypes)
                type.Value.SafeInvokeStaticMethod("Serialize", stream);

            return stream.ToArray();
        }

        public static void DeserializeStaticTypes(byte[] bytes, SerializationStream stream = null)
        {
            stream ??= CreateDeserializationStream(bytes);
            
            while (stream.HasBytesToRead)
            {
                var id = stream.Read<short>();
                var serializedBytes = stream.Read<byte[]>();
                if (!serializableStaticTypes.TryGetValue(id, out var targetType))
                {
                    Debug.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to find component with id {id}");
                    continue;
                }

                targetType.SafeInvokeStaticMethod("Deserialize", CreateDeserializationStream(serializedBytes));
            }
        }

        // EDITOR METHODS
        
#if UNITY_EDITOR

        [ContextMenu("Find serializable components")]
        private void FindSerializableComponents()
        {
            serializableObjects = FindObjectsOfType<SerializableObject>();

            EditorUtility.SetDirty(this);
            Debug.Log($"Founded {serializableObjects.Length} components");
        }

#endif
    }
}
#endif