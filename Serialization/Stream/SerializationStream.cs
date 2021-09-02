#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Rietmon.Extensions;
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.Serialization
{
    public class SerializationStream
    {
        public bool IsReading { get; }

        public bool IsWriting { get; }

        public int Version { get; set; }

        public bool HasBytesToRead => stream.Position < stream.Length;

        public bool IsEmpty => stream.Length == 0;

        private readonly MemoryStream stream;

        public SerializationStream()
        {
            IsReading = true;

            stream = new MemoryStream();
        }

        public SerializationStream(byte[] data)
        {
            IsWriting = true;

            stream = new MemoryStream(data);
        }

        public void Write(Type objectType, object value)
        {
            if (objectType == typeof(object))
                WriteValueType(value.GetType());
            
            switch (value)
            {
                case bool b: WriteByte(b ? (byte)1 : (byte)0); break;
                case byte b: WriteByte(b); break;
                case short s: WriteShort(s); break;
                case int i: WriteInt(i); break;
                case float f: WriteFloat(f); break;
                case double d: WriteDouble(d); break;
                case long l: WriteLong(l); break;
                case string s: WriteString(s); break;
                case ISerializable s: WriteSerializable(s); break;
#if UNITY_2020
                case Vector2 v: WriteVector2(v); break;
                case Vector3 v: WriteVector3(v); break;
                case Quaternion q: WriteQuaternion(q); break;
#endif
                case Array a: WriteArray(a); break;
                case IList l: WriteList(l); break;
                case IDictionary d: WriteDictionary(d); break;
            }
#if UNITY_2020
            Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(Read)}) Unsupported type {objectType}");
#endif
        }

        public void Write<T>(T obj)
        {
            if (typeof(T) == typeof(object))
                WriteValueType(obj.GetType());
            
            switch (obj)
            {
                case bool b: WriteByte(b ? (byte)1 : (byte)0); break;
                case byte b: WriteByte(b); break;
                case short s: WriteShort(s); break;
                case int i: WriteInt(i); break;
                case float f: WriteFloat(f); break;
                case double d: WriteDouble(d); break;
                case long l: WriteLong(l); break;
                case string s: WriteString(s); break;
                case ISerializable s: WriteSerializable(s); break;
#if UNITY_2020
                case Vector2 v: WriteVector2(v); break;
                case Vector2Int v: WriteVector2Int(v); break;
                case Vector3 v: WriteVector3(v); break;
                case Vector3Int v: WriteVector3Int(v); break;
                case Vector4 v: WriteVector4(v); break;
                case Quaternion q: WriteQuaternion(q); break;
#endif
                case Array a: WriteArray(a); break;
                case IList l: WriteList(l); break;
                case IDictionary d: WriteDictionary(d); break;
#if UNITY_2020
                default: Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(Write)}) Unsupported type {typeof(T)}"); break;
#endif
            }
        }

        private void WriteToStream(params byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteValueType(Type type)
        {
            Write(type.FullName);
        }

        private void WriteByte(byte value)
        {
            WriteToStream(value);
        }

        private void WriteShort(short value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteInt(int value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteFloat(float value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteDouble(double value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteLong(long value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteString(string value)
        {
            WriteShort((short)value.Length);
            WriteToStream(Encoding.UTF8.GetBytes(value));
        }

        private void WriteSerializable(ISerializable value)
        {
            value.Serialize(this);
        }
        
#if UNITY_2020
        private void WriteVector2(Vector2 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
        }
        
        private void WriteVector2Int(Vector2Int value)
        {
            WriteInt(value.x);
            WriteInt(value.y);
        }

        private void WriteVector3(Vector3 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
        }

        private void WriteVector3Int(Vector3Int value)
        {
            WriteInt(value.x);
            WriteInt(value.y);
            WriteInt(value.z);
        }

        private void WriteVector4(Vector4 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
            WriteFloat(value.w);
        }

        private void WriteQuaternion(Quaternion value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
            WriteFloat(value.w);
        }
#endif

        private void WriteArray(Array value)
        {
            WriteShort((short)value.Length);
            var elementsType = value.GetType().GetElementType();
            for (var i = 0; i < value.Length; i++)
            {
                var element = value.GetValue(i);
                Write(elementsType, element);
            }
        }

        private void WriteList(IList value)
        {
            WriteShort((short)value.Count);
            var elementType = value.GetType().GetGenericArguments()[0];
            foreach (var element in value)
            {
                Write(elementType, element);
            }
        }
        
        private void WriteDictionary(IDictionary value)
        {
            WriteShort((short)value.Count);
            var genericTypes = value.GetType().GetGenericArguments();
            foreach (var element in value)
            {
                var dictionaryEntry = (DictionaryEntry)element;
                
                Write(genericTypes[0], dictionaryEntry.Key);
                
                Write(genericTypes[1], dictionaryEntry.Value);
            }
        }

        public T Read<T>() => (T)Read(typeof(T));

        public object Read(Type type)
        {
            if (type == typeof(object))
                type = ReadType();
            
            if (type == typeof(bool)) return ReadByte() == 1;
            if (type == typeof(byte)) return ReadByte();
            if (type == typeof(short)) return ReadShort();
            if (type == typeof(int)) return ReadInt();
            if (type == typeof(float)) return ReadFloat();
            if (type == typeof(double)) return ReadDouble();
            if (type == typeof(long)) return ReadLong();
            if (type == typeof(string)) return ReadString();
            if (typeof(ISerializable).IsAssignableFrom(type)) return ReadSerializable(type);
#if UNITY_2020
            if (type == typeof(Vector2)) return ReadVector2();
            if (type == typeof(Vector3)) return ReadVector3();
            if (type == typeof(Quaternion)) return ReadQuaternion();
#endif
            if (type.IsArray) return ReadArray(type);
            if (typeof(IList).IsAssignableFrom(type)) return ReadList(type);
            if (typeof(IDictionary).IsAssignableFrom(type)) return ReadDictionary(type);

#if UNITY_2020
            Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(Read)}) Unsupported type {type}");
#endif
            return default;
        }

        private byte[] ReadFromStream(int count)
        {
            var bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        private Type ReadType()
        {
            var typeName = Read<string>();
            return Type.GetType(typeName);
        }

        private byte ReadByte()
        {
            return ReadFromStream(1).First();
        }

        private short ReadShort()
        {
            return BitConverter.ToInt16(ReadFromStream(2), 0);
        }

        private int ReadInt()
        {
            return BitConverter.ToInt32(ReadFromStream(4), 0);
        }

        private float ReadFloat()
        {
            return BitConverter.ToSingle(ReadFromStream(4), 0);
        }

        private double ReadDouble()
        {
            return BitConverter.ToDouble(ReadFromStream(8), 0);
        }

        private long ReadLong()
        {
            return BitConverter.ToInt64(ReadFromStream(8), 0);
        }

        private string ReadString()
        {
            var length = ReadShort();
            return Encoding.UTF8.GetString(ReadFromStream(length));
        }

        private ISerializable ReadSerializable(Type type)
        {
            var instance = (ISerializable)Activator.CreateInstance(type);
            instance.Deserialize(this);
            return instance;
        }

#if UNITY_2020
        private Vector2 ReadVector2()
        {
            return new Vector2
            {
                x = ReadFloat(),
                y = ReadFloat()
            };
        }

        private Vector3 ReadVector3()
        {
            return new Vector3
            {
                x = ReadFloat(),
                y = ReadFloat(),
                z = ReadFloat()
            };
        }

        private Quaternion ReadQuaternion()
        {
            return new Quaternion
            {
                x = ReadFloat(),
                y = ReadFloat(),
                z = ReadFloat(),
                w = ReadFloat()
            };
        }
#endif

        private Array ReadArray(Type type)
        {
            var length = ReadShort();
            var elementType = type.GetElementType();
            var array = Array.CreateInstance(elementType, length);
            
            for (var i = 0; i < length; i++)
            {
                var element = Read(elementType);
                array.SetValue(element, i);
            }
            return array;
        }

        private IList ReadList(Type type)
        {
            var length = ReadShort();
            var elementType = type.GetGenericArguments().First();
            var list = (IList)Activator.CreateInstance(type);
            
            for (var i = 0; i < length; i++)
            {
                var element = Read(elementType);
                list.Add(element);
            }
            return list;
        }
        
        private IDictionary ReadDictionary(Type type)
        {
            var length = ReadShort();
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            var dictionary = (IDictionary)Activator.CreateInstance(type);
            
            for (var i = 0; i < length; i++)
            {
                var key = Read(keyType);
                var value = Read(valueType);
                
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        public byte[] ToArray() => stream.ToArray();

        public SerializationStream CreateSerializationSubStream() =>
            new SerializationStream
            {
                Version = Version
            };

        public SerializationStream CreateDeserializationSubStream(byte[] bytes) =>
            new SerializationStream(bytes)
            {
                Version = Version
            };

        ~SerializationStream()
        {
            stream.Dispose();
        }
    }
}
#endif