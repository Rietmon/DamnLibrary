#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.Reflection;
using DamnLibrary.Debugs;
using DamnLibrary.Serializations.Serializables;
using DamnLibrary.Utilities.Extensions;
using UnityEngine;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        public void WriteWithReflection(object value)
        {
            if (value == null)
            {
                UniversalDebugger.LogError($"[{nameof(SerializationStream)}] ({nameof(WriteWithReflection)}) Value is null!");
                return;
            }
            
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

#if ENABLE_SERIALIZATION_CHECKS
            if (layoutSettings == null && !type.Namespace.IsNullOrEmpty() && !type.Namespace.Contains("Unity") && !type.Namespace.Contains("System."))
            {
                UniversalDebugger.LogWarning($"[{nameof(SerializationStream)}] ({nameof(WriteAnyWithReflection)}) " +
                                             $"Writing with reflection type {type.FullName} which is not have {nameof(SerializableLayoutAttribute)}." +
                                             $"Basically it's ok but you can get unexpected results be cause serializing will write fields one by one in order of declaration." +
                                             $"This is not always safe for models which is often changed.");
            }
#endif
            
            if (layoutSettings is { WrapToContainer: true })
                BeginContainer();
            
            var useKeyValuePair = layoutSettings?.UseKeyValuePair ?? false;
            
            foreach (var property in type.GetProperties(flags))
            {
                if (property.SetMethod.IsStatic || Attribute.IsDefined(property, typeof(SerializeIgnoreAttribute)))
                    continue;
                
                if (!property.SetMethod.IsPublic && !Attribute.IsDefined(property, typeof(SerializeIncludeAttribute)))
                    continue;
                
                if (useKeyValuePair)
                {
                    try
                    {
                        var propertyValue = property.GetValue(value);
                        if (propertyValue == null)
                            continue;
                    
                        Write(property.PropertyType);
                        WriteString(property.Name);
                        WriteWithReflection(propertyValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(WriteAnyWithReflection)}) " +
                                       $"Error while writing property {property.Name} of type {property.PropertyType} " +
                                       $"of object {value} of type {type}:\n{e}");
                    }
                }
                else
                    WriteWithReflection(property.GetValue(value));
            }
            
            foreach (var field in type.GetFields(flags))
            {
                if (field.IsStatic || Attribute.IsDefined(field, typeof(SerializeIgnoreAttribute)))
                    continue;
                
                if (!field.IsPublic && !Attribute.IsDefined(field, typeof(SerializeIncludeAttribute)))
                    continue;
                
                if (useKeyValuePair)
                {
                    try
                    {
                        var fieldValue = field.GetValue(value);
                        if (fieldValue == null)
                            continue;

                        Write(field.FieldType);
                        WriteString(field.Name);
                        WriteWithReflection(fieldValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(WriteAnyWithReflection)}) " +
                                       $"Error while writing property {field.Name} of type {field.FieldType} " +
                                       $"of object {value} of type {type}:\n{e}");
                    }
                }
                else
                    WriteWithReflection(field.GetValue(value));
            }

            if (layoutSettings is { WrapToContainer: true })
                EndContainer();
        }
    }
}
#endif