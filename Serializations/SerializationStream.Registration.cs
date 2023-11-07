using System;
using System.Collections.Generic;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private static Dictionary<Type, (Action<SerializationStream>, Action<SerializationStream>)> RegisteredTypes { get; } = new();
        
        public static void RegisterTypeForSerialization(Type type, 
            Action<SerializationStream> write, 
            Action<SerializationStream> read) =>
            RegisteredTypes[type] = (write, read);
        
        public static bool IsTypeRegisteredForSerialization(Type type) =>
            RegisteredTypes.ContainsKey(type);
        
        public static void UnregisterTypeForSerialization(Type type) =>
            RegisteredTypes.Remove(type);

        internal static bool TryGetSerializationActions(Type type,
            out (Action<SerializationStream>, Action<SerializationStream>) result) =>
            RegisteredTypes.TryGetValue(type, out result);

    }
}