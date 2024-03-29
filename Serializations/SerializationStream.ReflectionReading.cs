#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DamnLibrary.Debugs;
using DamnLibrary.Serializations.Serializables;
using DamnLibrary.Utilities.Extensions;
using UnityEngine;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        public T ReadWithReflection<T>() => (T)ReadWithReflection(typeof(T));

        public object ReadWithReflection(Type type) =>
            type switch
            {
                not null when type == boolType => ReadBool(),
                not null when type == sbyteType => ReadSByte(),
                not null when type == byteType => ReadByte(),
                not null when type == shortType => ReadShort(),
                not null when type == ushortType => ReadUShort(),
                not null when type == intType => ReadInt(),
                not null when type == uintType => ReadUInt(),
                not null when type == longType => ReadLong(),
                not null when type == ulongType => ReadULong(),
                not null when type == charType => ReadChar(),
                not null when type == floatType => ReadFloat(),
                not null when type == doubleType => ReadDouble(),
                not null when type == stringType => ReadString(),
                not null when type == decimalType => ReadDecimal(),
                not null when type.IsArray => ReadArrayWithReflection(type),
                not null when iListType.IsAssignableFrom(type) => ReadListWithReflection(type),
                not null when iDictionaryType.IsAssignableFrom(type) => ReadDictionaryWithReflection(type),
                not null when iSerializableType.IsAssignableFrom(type) => ReadSerializableWithReflection(type),
                not null when type == dateTimeType => ReadDateTimeWithReflection(),
                not null when type == typeType => ReadType(),
                _ => SolveUncommonType(type)
            };

        private object SolveUncommonType(Type type) => 
            TryGetSerializationActions(type, out var methods) 
                ? methods.Item2(this) 
                : ReadAnyWithReflection(type);

        public Array ReadArrayWithReflection(Type type)
        {
            var length = ReadInt();
            var arrayElementsType = type.GetElementType()!;
            var result = Array.CreateInstance(arrayElementsType, length);
            for (var i = 0; i < length; i++)
                result.SetValue(ReadWithReflection(arrayElementsType), i);
            return result;
        }

        public Array ReadArrayByElementTypeWithReflection(Type elementType)
        {
            var length = ReadInt();
            var result = Array.CreateInstance(elementType, length);
            for (var i = 0; i < length; i++)
                result.SetValue(ReadWithReflection(elementType), i);
            return result;
        }

        public IList ReadListWithReflection(Type type)
        {
            var count = ReadInt();
            var listElementType = type.GenericTypeArguments.FirstOrDefault();
            var result = (IList)Activator.CreateInstance(type)!;
            for (var i = 0; i < count; i++)
                result.Add(ReadWithReflection(listElementType));
            return result;
        }

        public IDictionary ReadDictionaryWithReflection(Type type)
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            
            var keys = ReadArrayByElementTypeWithReflection(keyType);
            var values = ReadArrayByElementTypeWithReflection(valueType);
            
            var result = (IDictionary)Activator.CreateInstance(type)!;
            for (var i = 0; i < keys.Length; i++)
                result.Add(keys.GetValue(i)!, values.GetValue(i));
            return result;
        }
        
        public ISerializable ReadSerializableWithReflection(Type type)
        {
            var result = (ISerializable)Activator.CreateInstance(type)!;
            result.Deserialize(this);
            return result;
        }
        
        public DateTime ReadDateTimeWithReflection() => new(ReadLong());

        public object ReadBoxed()
        {
            var type = ReadType();
            return ReadWithReflection(type);
        }
        
        public T ReadAnyWithReflection<T>() => (T)ReadAnyWithReflection(typeof(T));

        public object ReadAnyWithReflection(Type type)
        {
            var layoutSettings = (SerializableLayoutAttribute)Attribute.GetCustomAttribute(type, typeof(SerializableLayoutAttribute));
            
            var stream = this;
            if (layoutSettings is { WrapToContainer: true })
            {
                stream = ReadContainer();
                if (layoutSettings is { UseKeyValuePair: true })
                    return Internal_ReadKeyValuesWithReflection(stream, type);
            }
            
#if ENABLE_SERIALIZATION_CHECKS
            if (!type.Namespace.IsNullOrEmpty() && !type.Namespace.Contains("Unity") && !type.Namespace.Contains("System."))
            {
                UniversalDebugger.LogWarning($"[{nameof(SerializationStream)}] ({nameof(ReadAnyWithReflection)}) " +
                                             $"Reading with reflection type {type.FullName} which is not have {nameof(SerializableLayoutAttribute)}." +
                                             $"Basically it's ok but you can get unexpected results be cause serializing will write fields one by one in order of declaration." +
                                             $"This is always safe for models which is often changed.");
            }
#endif
            return Internal_ReadAnyWithReflection(stream, type);
        }

        private static object Internal_ReadKeyValuesWithReflection(SerializationStream stream, Type type)
        {
            var result = Activator.CreateInstance(type);
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            var keyValues = new List<(string, object)>();
            while (stream.HasToRead)
            {
                var pairType = stream.ReadType();
                var pairName = stream.ReadString();
                var pairValue = stream.ReadWithReflection(pairType);
                keyValues.Add((pairName, pairValue));
            }

            foreach (var (name, value) in keyValues)
            {
                var property = type.GetProperty(name, flags);
                if (property != null)
                {
                    property.SetValue(result, value);
                    continue;
                }
                
                var field = type.GetField(name, flags);
                if (field != null)
                    field.SetValue(result, value);
            }

            return result;
        }

        private static object Internal_ReadAnyWithReflection(SerializationStream stream, Type type)
        {
            var result = Activator.CreateInstance(type);
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            
            foreach (var property in type.GetProperties(flags))
            {
                if (property.GetMethod.IsStatic || Attribute.IsDefined(property, typeof(SerializeIgnoreAttribute)))
                    continue;
                
                if (!property.GetMethod.IsPublic && !Attribute.IsDefined(property, typeof(SerializeIncludeAttribute)))
                    continue;

                var value = stream.ReadWithReflection(property.PropertyType);
                property.SetValue(result, value);
            }
            
            foreach (var field in type.GetFields(flags))
            {
                if (field.IsStatic || Attribute.IsDefined(field, typeof(SerializeIgnoreAttribute)))
                    continue;
                
                if (!field.IsPublic && !Attribute.IsDefined(field, typeof(SerializeIncludeAttribute)))
                    continue;

                var value = stream.ReadWithReflection(field.FieldType);
                field.SetValue(result, value);
            }

            return result;
        }
    }
}
#endif