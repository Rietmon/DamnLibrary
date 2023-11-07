using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private static readonly Type boolType = typeof(bool);
        private static readonly Type sbyteType = typeof(sbyte);
        private static readonly Type byteType = typeof(byte);
        private static readonly Type shortType = typeof(short);
        private static readonly Type ushortType = typeof(ushort);
        private static readonly Type intType = typeof(int);
        private static readonly Type uintType = typeof(uint);
        private static readonly Type longType = typeof(long);
        private static readonly Type ulongType = typeof(ulong);
        private static readonly Type charType = typeof(char);
        private static readonly Type floatType = typeof(float);
        private static readonly Type doubleType = typeof(double);
        private static readonly Type stringType = typeof(string);
        private static readonly Type decimalType = typeof(decimal);
        private static readonly Type iSerializableType = typeof(ISerializable);
        private static readonly Type iListType = typeof(IList);
        private static readonly Type iDictionaryType = typeof(IDictionary);
        private static readonly Type dateTimeType = typeof(DateTime);
        private static readonly Type typeType = typeof(Type);
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
                _ => Internal_ReadWithReflection(type)
            };

        public object ReadArrayWithReflection(Type type)
        {
            var length = ReadInt();
            var arrayElementsType = type.GetElementType()!;
            var result = Array.CreateInstance(arrayElementsType, length);
            for (var i = 0; i < length; i++)
                result.SetValue(ReadWithReflection(arrayElementsType), i);
            return result;
        }

        public object ReadListWithReflection(Type type)
        {
            var count = ReadInt();
            var listElementType = type.GenericTypeArguments.FirstOrDefault();
            var result = (IList)Activator.CreateInstance(type)!;
            for (var i = 0; i < count; i++)
                result.Add(ReadWithReflection(listElementType));
            return result;
        }

        public object ReadDictionaryWithReflection(Type type)
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            
            var keys = (Array)ReadArrayWithReflection(keyType);
            var values = (Array)ReadArrayWithReflection(valueType);
            
            var result = (IDictionary)Activator.CreateInstance(type)!;
            for (var i = 0; i < keys.Length; i++)
                result.Add(keys.GetValue(i)!, values.GetValue(i));
            return result;
        }
        
        public object ReadSerializableWithReflection(Type type)
        {
            var result = (ISerializable)Activator.CreateInstance(type)!;
            result.Deserialize(this);
            return result;
        }
        
        public object ReadDateTimeWithReflection() => new DateTime(ReadLong());
        
        public void WriteWithReflection<T>(T value) => WriteWithReflection(value, typeof(T));

        private void WriteWithReflection<T>(T value, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var layoutSettings = (SerializeLayoutAttribute)Attribute.GetCustomAttribute(type, typeof(SerializeLayoutAttribute));
            
            if (layoutSettings is { WrapToContainer: true })
                BeginContainer();
            
            var useKeyValuePair = layoutSettings?.UseKeyValuePair ?? false;
            
            foreach (var property in type.GetProperties(flags))
            {
                if (!Attribute.IsDefined(property, typeof(SerializeIncludeAttribute)))
                    continue;
                
                Write(property.PropertyType);
                if (useKeyValuePair)
                    WriteKeyValuePair(property.Name, property.GetValue(value));
                else
                    Write(property.GetValue(value));
            }
            
            foreach (var field in type.GetFields(flags))
            {
                if (!Attribute.IsDefined(field, typeof(SerializeIncludeAttribute)))
                    continue;
                
                if (useKeyValuePair)
                {
                    Write(field.FieldType);
                    WriteKeyValuePair(field.Name, field.GetValue(value));
                }
                else
                    Write(field.GetValue(value));
            }

            if (layoutSettings is { WrapToContainer: true })
                EndContainer();
        }

        private object Internal_ReadWithReflection(Type type)
        {
            var layoutSettings = (SerializeLayoutAttribute)Attribute.GetCustomAttribute(type, typeof(SerializeLayoutAttribute));
            
            var stream = this;
            if (layoutSettings is { WrapToContainer: true })
            {
                stream = ReadContainer();
                if (layoutSettings is { UseKeyValuePair: true })
                    return Internal_ReadKeyValuesWithReflection(stream, type);
            }
            
            return Internal_ReadWithReflection(stream, type);
        }

        private static object Internal_ReadKeyValuesWithReflection(SerializationStream stream, Type type)
        {
            var result = Activator.CreateInstance(type);
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            var keyValues = new List<(string, object)>();
            while (stream.HasToRead)
            {
                var typeName = stream.ReadType();
                keyValues.Add(((string, object))stream.ReadKeyValuePair(stringType, typeName));
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

        private static object Internal_ReadWithReflection(SerializationStream stream, Type type)
        {
            return default;
        }
    }
}