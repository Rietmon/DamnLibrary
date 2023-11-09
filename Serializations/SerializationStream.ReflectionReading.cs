using System;
using System.Collections;
using System.Linq;
using DamnLibrary.Serializations.Serializables;

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
    }
}