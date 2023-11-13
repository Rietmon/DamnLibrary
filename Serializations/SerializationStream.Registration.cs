#if ENABLE_SERIALIZATION
using System;
using System.Collections.Generic;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private static Dictionary<Type, (Action<object, SerializationStream>, Func<SerializationStream, object>)> RegisteredTypes { get; } = new();
        
        public static void RegisterTypeForSerialization(Type type, 
            Action<object, SerializationStream> write, 
            Func<SerializationStream, object> read) =>
            RegisteredTypes[type] = (write, read);
        
        public static bool IsTypeRegisteredForSerialization(Type type) =>
            RegisteredTypes.ContainsKey(type);
        
        public static void UnregisterTypeForSerialization(Type type) =>
            RegisteredTypes.Remove(type);

        internal static bool TryGetSerializationActions(Type type,
            out (Action<object, SerializationStream>, Func<SerializationStream, object>) result) =>
            RegisteredTypes.TryGetValue(type, out result);
    }
}
#endif