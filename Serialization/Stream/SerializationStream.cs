#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rietmon.Extensions;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace Rietmon.Serialization
{
    public class SerializationStream
    {
        private static readonly Dictionary<Type, Func<object, byte[]>> customSerialization =
            new Dictionary<Type, Func<object, byte[]>>();

        private static readonly Dictionary<Type, Func<SerializationStream, object>> customDeserialization =
            new Dictionary<Type, Func<SerializationStream, object>>();

        private static readonly List<Type> dynamicTypes = new List<Type>();
        
        public bool IsReading { get; }

        public bool IsWriting { get; }

        public int Version { get; set; }

        public bool HasBytesToRead => stream.Position < stream.Length;

        public bool IsEmpty => stream.Length == 0;

        private readonly MemoryStream stream;

        public SerializationStream()
        {
            IsWriting = true;

            stream = new MemoryStream();
        }

        public SerializationStream(byte[] data)
        {
            IsReading = true;

            stream = new MemoryStream(data);
        }

        public void Write<T>(T obj, Type realType = null)
        {
            if (!IsWriting)
                return;

            if (realType != null && IsDynamicType(realType) ||
                realType == null && IsDynamicType(typeof(T)))
                WriteValueType(obj.GetType());
            
            switch (obj)
            {
                case bool b: WriteByte(b ? (byte)1 : (byte)0); return;
                case byte b: WriteByte(b); return;
                case sbyte b: WriteSByte(b); return;
                case short s: WriteShort(s); return;
                case ushort s: WriteUShort(s); return;
                case int i: WriteInt(i); return;
                case uint i: WriteUInt(i); return;
                case float f: WriteFloat(f); return;
                case double d: WriteDouble(d); return;
                case long l: WriteLong(l); return;
                case ulong l: WriteULong(l); return;
                case decimal l: WriteDecimal(l); return;
                case string s: WriteString(s); return;
                case ISerializable s: WriteSerializable(s); return;
#if UNITY_5_3_OR_NEWER 
                case Vector2 v: WriteVector2(v); return;
                case Vector2Int v: WriteVector2Int(v); return;
                case Vector3 v: WriteVector3(v); return;
                case Vector3Int v: WriteVector3Int(v); return;
                case Vector4 v: WriteVector4(v); return;
                case Quaternion q: WriteQuaternion(q); return;
#endif
                case Array a: WriteArray(a); return;
                case IList l: WriteList(l); return;
                case IDictionary d: WriteDictionary(d); return;
            }

            var targetSerializationType = obj.GetType();
            if (customSerialization.TryGetValue(targetSerializationType, out var method))
                WriteToStream(method.Invoke(obj));
#if UNITY_5_3_OR_NEWER 
            else
                Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(Write)}) Unsupported type {typeof(T)}");
#endif
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
        
        private void WriteSByte(sbyte value)
        {
            WriteToStream((byte)(value + 128));
        }

        private void WriteShort(short value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteUShort(ushort value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteInt(int value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }
        private void WriteUInt(uint value)
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

        private void WriteULong(ulong value)
        {
            WriteToStream(BitConverter.GetBytes(value));
        }

        private void WriteDecimal(decimal value)
        {
            WriteToStream(DecimalUtilities.GetBytes(value));
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
        
#if UNITY_5_3_OR_NEWER 
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
            var elementType = value.GetType().GetElementType();
            for (var i = 0; i < value.Length; i++)
            {
                var element = value.GetValue(i);
                Write(element, elementType);
            }
        }

        private void WriteList(IList value)
        {
            WriteShort((short)value.Count);
            var elementType = value.GetType().GetGenericArguments()[0];
            foreach (var element in value)
            {
                Write(element, elementType);
            }
        }
        
        private void WriteDictionary(IDictionary value)
        {
            WriteShort((short)value.Count);
            var genericTypes = value.GetType().GetGenericArguments();
            foreach (var element in value)
            {
                var dictionaryEntry = (DictionaryEntry)element;
                
                Write(dictionaryEntry.Key, genericTypes[0]);
                
                Write(dictionaryEntry.Value, genericTypes[1]);
            }
        }

        public T Read<T>() => (T)Read(typeof(T));

        public object Read(Type type)
        {
            if (!IsReading)
                return null;
            
            if (IsDynamicType(type))
                type = ReadType();
            
            if (type == typeof(bool)) return ReadByte() == 1;
            if (type == typeof(byte)) return ReadByte();
            if (type == typeof(sbyte)) return ReadSByte();
            if (type == typeof(short)) return ReadShort();
            if (type == typeof(ushort)) return ReadUShort();
            if (type == typeof(int)) return ReadInt();
            if (type == typeof(uint)) return ReadUInt();
            if (type == typeof(float)) return ReadFloat();
            if (type == typeof(double)) return ReadDouble();
            if (type == typeof(long)) return ReadLong();
            if (type == typeof(ulong)) return ReadULong();
            if (type == typeof(decimal)) return ReadDecimal();
            if (type == typeof(string)) return ReadString();
            if (typeof(ISerializable).IsAssignableFrom(type)) return ReadSerializable(type);
#if UNITY_5_3_OR_NEWER 
            if (type == typeof(Vector2)) return ReadVector2();
            if (type == typeof(Vector2Int)) return ReadVector2Int();
            if (type == typeof(Vector3)) return ReadVector3();
            if (type == typeof(Vector3Int)) return ReadVector3Int();
            if (type == typeof(Vector4)) return ReadVector4();
            if (type == typeof(Quaternion)) return ReadQuaternion();
#endif
            if (type.IsArray) return ReadArray(type);
            if (typeof(IList).IsAssignableFrom(type)) return ReadList(type);
            if (typeof(IDictionary).IsAssignableFrom(type)) return ReadDictionary(type);

            if (customDeserialization.TryGetValue(type, out var method)) return method.Invoke(this);

#if UNITY_5_3_OR_NEWER 
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
            var typeName = ReadString();
            return Type.GetType(typeName);
        }

        private byte ReadByte()
        {
            return ReadFromStream(1).First();
        }

        private sbyte ReadSByte()
        {
            return (sbyte)((sbyte)ReadFromStream(1).First() - 128);
        }

        private short ReadShort()
        {
            return BitConverter.ToInt16(ReadFromStream(2), 0);
        }

        private ushort ReadUShort()
        {
            return BitConverter.ToUInt16(ReadFromStream(2), 0);
        }

        private int ReadInt()
        {
            return BitConverter.ToInt32(ReadFromStream(4), 0);
        }

        private uint ReadUInt()
        {
            return BitConverter.ToUInt32(ReadFromStream(4), 0);
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

        private ulong ReadULong()
        {
            return BitConverter.ToUInt64(ReadFromStream(8), 0);
        }

        private object ReadDecimal()
        {
            return DecimalUtilities.FromBytes(ReadFromStream(16));
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

#if UNITY_5_3_OR_NEWER 
        private Vector2 ReadVector2()
        {
            return new Vector2
            {
                x = ReadFloat(),
                y = ReadFloat()
            };
        }
       
        private Vector2Int ReadVector2Int()
        {
            return new Vector2Int
            {
                x = ReadInt(),
                y = ReadInt()
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

        private Vector3Int ReadVector3Int()
        {
            return new Vector3Int
            {
                x = ReadInt(),
                y = ReadInt(),
                z = ReadInt()
            };
        }

        private Vector4 ReadVector4()
        {
            return new Vector4
            {
                x = ReadFloat(),
                y = ReadFloat(),
                z = ReadFloat(),
                w = ReadFloat()
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

        public static void AddCustomSerialization(Type type, Func<object, byte[]> serializationMethod, 
            Func<SerializationStream, object> deserializationMethod)
        {
            customSerialization.Add(type, serializationMethod);
            customDeserialization.Add(type, deserializationMethod);
        }

        public static void RemoveCustomSerialization(Type type)
        {
            customSerialization.Remove(type);
            customDeserialization.Remove(type);
        }

        public static void AddDynamicType(Type type) => 
            dynamicTypes.AddIfNotContains(type);
        
        public static void RemoveDynamicType(Type type) => 
            dynamicTypes.Remove(type);

        public static bool IsDynamicType(Type type) => type == typeof(object) || dynamicTypes.Contains(type);
    }
}
#endif