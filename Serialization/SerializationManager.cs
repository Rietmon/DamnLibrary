#if ENABLE_SERIALIZATION
using System;
using System.Collections.Generic;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
#if UNITY_5_3_OR_NEWER 
using DamnLibrary.Behaviours;
using DamnLibrary.Management;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#endif

namespace DamnLibrary.Serialization
{
#if UNITY_5_3_OR_NEWER 
    public class SerializationManager : SingletonBehaviour<SerializationManager>
#else
    public static class SerializationManager
#endif
    {
#if UNITY_5_3_OR_NEWER 
        public static short Version { get => Instance.version; set => Instance.version = value; }
#else
        public static short Version { get; set; } = 1;
#endif

        private static readonly Dictionary<short, Type> serializableStaticTypes = new();

#if UNITY_5_3_OR_NEWER 
        [SerializeField] private short version = 1;
    
        [SerializeField] private SerializableObject[] serializableObjects;
#endif

        public static void FindStaticSerializable()
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
            var stream = new SerializationStream { Version = Version };
            stream.Write(Version);
            return stream;
        }

        public static SerializationStream CreateDeserializationStream(byte[] bytes)
        {
            var stream = new SerializationStream(bytes);
            stream.Version = stream.Read<short>();
            return stream;
        }

#if UNITY_5_3_OR_NEWER 
        public static byte[] SerializeComponents(SerializationStream stream = null)
        {
            if (Instance == null)
            {
                UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize at the {SceneManager.ActiveScene.name} scene, because there is no serialization component!");
                return Array.Empty<byte>();
            }
            
            stream ??= CreateSerializationStream();

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(SerializeComponents)}) Unable to serialize at the {SceneManager.ActiveScene.name} scene, because there is no serializable objects!");
                return Array.Empty<byte>();
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

        public static void DeserializeComponents(byte[] bytes, SerializationStream stream = null)
        {
            if (Instance == null)
            {
                UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because there is no serialization component!");
                return;
            }

            if (bytes == null || bytes.Length == 0)
            {
                UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because byte is null or length is equal 0!");
                return;
            }
            
            stream ??= CreateDeserializationStream(bytes);

            var objects = Instance.serializableObjects;

            if (objects == null || objects.Length == 0)
            {
                UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to deserialize at the {SceneManager.ActiveScene.name} scene, because there is no serializable objects!");
                return;
            }

            while (stream.HasBytesToRead)
            {
                var id = stream.Read<short>();
                var componentBytes = stream.Read<byte[]>();
                var component = Instance.serializableObjects.FindOrDefault((c) => c.SerializableId == id);
                if (!component)
                {
                    UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeComponents)}) Unable to find component with id {id} at the {SceneManager.ActiveScene.name} scene");
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
                    UniversalDebugger.LogError($"[{nameof(Serialization)}] ({nameof(DeserializeStaticTypes)}) Unable to find static type with id {id}");
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
            UniversalDebugger.Log($"[{nameof(Serialization)}] ({nameof(FindSerializableComponents)}) Found {serializableObjects.Length} components");
        }

#endif
    }
}
#endif