using System;
using System.Collections;
using System.Reflection;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        public void WriteWithReflection(object value)
        {
            var type = value.GetType();
            switch (type)
            {
                case not null when type == boolType: WriteBool((bool)value); return;
                case not null when type == sbyteType: WriteSByte((sbyte)value); return;
                case not null when type == byteType: WriteByte((byte)value); return;
                case not null when type == shortType: WriteShort((short)value); return;
                case not null when type == ushortType: WriteUShort((ushort)value); return;
                case not null when type == intType: WriteInt((int)value); return;
                case not null when type == uintType: WriteUInt((uint)value); return;
                case not null when type == longType: WriteLong((long)value); return;
                case not null when type == ulongType: WriteULong((ulong)value); return;
                case not null when type == charType: WriteChar((char)value); return;
                case not null when type == floatType: WriteFloat((float)value); return;
                case not null when type == doubleType: WriteDouble((double)value); return;
                case not null when type == stringType: WriteString((string)value); return;
                case not null when type == decimalType: WriteDecimal((decimal)value); return;
                case not null when type.IsArray: WriteArrayWithReflection((Array)value); return;
                case not null when iListType.IsAssignableFrom(type): WriteListWithReflection((IList)value); return;
                case not null when iDictionaryType.IsAssignableFrom(type): WriteDictionaryWithReflection((IDictionary)value); return;
                case not null when iSerializableType.IsAssignableFrom(type): WriteSerializable((ISerializable)value); return;
                case not null when type == dateTimeType: WriteDateTime((DateTime)value); return;
                case not null when type == typeType: WriteType((Type)value); return;
            }
            
            if (TryGetSerializationActions(type, out var methods))
            {
                methods.Item1(value, this);
                return;
            }

            WriteAnyWithReflection(value, type);
        }

        public void WriteIEnumerableWithReflection(IEnumerable enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                Write(value);
        }

        public void WriteNonGenericIEnumerableWithReflection(IEnumerable enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                WriteBoxed(value);
        }

        public void WriteArrayWithReflection(Array array) => 
            WriteIEnumerableWithReflection(array, array.Length);

        public void WriteListWithReflection(IList list) => 
            WriteIEnumerableWithReflection(list, list.Count);

        public void WriteDictionaryWithReflection(IDictionary dictionary)
        {
            WriteIEnumerableWithReflection(dictionary.Keys, dictionary.Count);
            WriteIEnumerableWithReflection(dictionary.Values, dictionary.Count);
        }
        
        public void WriteAnyWithReflection<T>(T value) => WriteAnyWithReflection(value, typeof(T));

        private void WriteAnyWithReflection<T>(T value, Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var layoutSettings = (SerializableLayoutAttribute)Attribute.GetCustomAttribute(type, typeof(SerializableLayoutAttribute));
            
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
    }
}