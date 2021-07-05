#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
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
            IsWriting = false;

            stream = new MemoryStream();
        }

        public SerializationStream(byte[] data)
        {
            IsReading = false;
            IsWriting = true;

            stream = new MemoryStream(data);
        }

        public void Write<T>(T obj)
        {
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
#if UNITY_2020
                case Vector2 v: WriteVector2(v); break;
                case Vector3 v: WriteVector3(v); break;
                case Quaternion q: WriteQuaternion(q); break;
#endif
                case Array a: WriteArray(a); break;
                case IList l: WriteList(l); break;
#if UNITY_2020
                default: Debug.LogError($"[{nameof(SerializationStream)}] ({nameof(Write)}) Unsupported type {typeof(T)}"); break;
#endif
            }
        }

        private void WriteToStream(params byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
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
        
#if UNITY_2020
        private void WriteVector2(Vector2 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
        }

        private void WriteVector3(Vector3 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
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
            for (var i = 0; i < value.Length; i++)
                Write(value.GetValue(i));
        }

        private void WriteList(IList value)
        {
            WriteShort((short)value.Count);
            foreach (var element in value)
                Write(element);
        }

        public T Read<T>() => (T)Read(typeof(T));

        private object Read(Type type)
        {
            if (type == typeof(bool)) return ReadByte() == 1;
            if (type == typeof(byte)) return ReadByte();
            if (type == typeof(short)) return ReadShort();
            if (type == typeof(int)) return ReadInt();
            if (type == typeof(float)) return ReadFloat();
            if (type == typeof(double)) return ReadDouble();
            if (type == typeof(long)) return ReadLong();
#if UNITY_2020
            if (type == typeof(Vector2)) return ReadVector2();
            if (type == typeof(Vector3)) return ReadVector3();
            if (type == typeof(Quaternion)) return ReadQuaternion();
#endif
            if (type.IsArray) return ReadArray(type);
            if (typeof(IList).IsAssignableFrom(type)) return ReadList(type);
            if (type == typeof(string)) return ReadString();

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
                array.SetValue(Read(elementType), i);
            return array;
        }

        private IList ReadList(Type type)
        {
            var length = ReadShort();
            var elementType = type.GetGenericArguments().First();
            var list = (IList)Activator.CreateInstance(type);
            for (var i = 0; i < length; i++)
                list.Add(Read(elementType));
            return list;
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