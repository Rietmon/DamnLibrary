using System;
using System.Collections.Generic;
using System.Reflection;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        
        
        public void WriteWithReflection<T>(T value) => WriteWithReflection(value, typeof(T));

        private void WriteWithReflection<T>(T value, Type type)
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

        private object Internal_ReadWithReflection(Type type)
        {
            var layoutSettings = (SerializableLayoutAttribute)Attribute.GetCustomAttribute(type, typeof(SerializableLayoutAttribute));
            
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