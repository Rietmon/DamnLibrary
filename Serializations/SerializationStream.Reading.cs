#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DamnLibrary.Debugs;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public unsafe partial class SerializationStream
    {
        private BinaryReader Reader { get; }

        public T Read<T>() where T : new()
        { 
            var value = default(T);
            Read(ref value);
            return value;
        }

        public void Read<T>(ref T value) where T : new()
        {
            switch (value)
            {
                case bool: value = (T)(object)ReadBool(); return;
                case sbyte: value = (T)(object)ReadSByte(); return;
                case byte: value = (T)(object)ReadByte(); return;
                case short: value = (T)(object)ReadShort(); return;
                case ushort: value = (T)(object)ReadUShort(); return;
                case int: value = (T)(object)ReadInt(); return;
                case uint: value = (T)(object)ReadUInt(); return;
                case long: value = (T)(object)ReadLong(); return;
                case ulong: value = (T)(object)ReadULong(); return;
                case float: value = (T)(object)ReadFloat(); return;
                case double: value = (T)(object)ReadDouble(); return;
                case decimal: value = (T)(object)ReadDecimal(); return;
                case char: value = (T)(object)ReadChar(); return;
                case string: value = (T)(object)ReadString(); return;
                case Array: throw new Exception("Use ReadArray instead of Read<T[]>"!);
                case IList: case IList<T>: throw new Exception("Use ReadList instead of Read<IList>"!);
                case IDictionary: throw new Exception("Use ReadDictionary instead of Read<IDictionary>"!);
                case ISerializable: value = ReadSerializable<T>(); return;
                case DateTime: value = (T)(object)ReadDateTime(); return;
                case Type: value = (T)(object)ReadType(); return;
            }
        }

        public T ReadUnmanaged<T>() where T : unmanaged
        {
            var size = sizeof(T);
            var buffer = stackalloc T[1];
            var read = Reader.Read(new Span<byte>(buffer, size));
            if (read != size)
                return default;
            
            return *buffer;
        }

        public bool ReadBool() => Reader.ReadBoolean();

        public sbyte ReadSByte() => Reader.ReadSByte();

        public byte ReadByte() => Reader.ReadByte();

        public short ReadShort() => Reader.ReadInt16();

        public ushort ReadUShort() => Reader.ReadUInt16();

        public int ReadInt() => Reader.ReadInt32();

        public uint ReadUInt() => Reader.ReadUInt32();

        public long ReadLong() => Reader.ReadInt64();

        public ulong ReadULong() => Reader.ReadUInt64();

        public char ReadChar() => Reader.ReadChar();
        
        public float ReadFloat() => Reader.ReadSingle();
        
        public double ReadDouble() => Reader.ReadDouble();
        
        public decimal ReadDecimal() => Reader.ReadDecimal();

        public string ReadString() => Reader.ReadString();

        public T[] ReadUnmanagedArray<T>()
        {
            var length = ReadInt();
            if (length > ushort.MaxValue)
            {
                UniversalDebugger.LogError($"[{nameof(SerializationStream)}] ({nameof(ReadUnmanagedArray)}) Length is too big [{length.ToString()}], probably it's error!");
                return default;
            }
            var sizeOfElement = Unsafe_SizeOf<T>();
            var size = length * sizeOfElement;
            var buffer = stackalloc byte[size];
            var read = Reader.Read(new Span<byte>(buffer, size));
            if (read != size)
                return default;

            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
#if !UNITY_5_3_OR_NEWER
                result[i] = Unsafe_ReadNetCore<T>(buffer, i, sizeOfElement);
#else
                result[i] = Unsafe_ReadUnity<T>(buffer, i);
#endif
            }
            return result;
        }

        public T[] ReadManagedArray<T>()
        {
            var length = ReadInt();
            var array = new T[length];
            for (var i = 0; i < length; i++)
                array[i] = ReadWithReflection<T>();
            return array;
        }

        public T[] ReadArray<T>() => RuntimeHelpers.IsReferenceOrContainsReferences<T>() 
            ? ReadManagedArray<T>() 
            : ReadUnmanagedArray<T>();

        public List<T> ReadList<T>() => RuntimeHelpers.IsReferenceOrContainsReferences<T>() 
            ? new List<T>(ReadManagedArray<T>()) 
            : new List<T>(ReadUnmanagedArray<T>());

        public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>()
        {
            var result = new Dictionary<TKey, TValue>();
            var keys = ReadArray<TKey>();
            var values = ReadArray<TValue>();
            for (var i = 0; i < keys.Length; i++)
                result.Add(keys[i], values[i]);
            return result;
        }

        public T ReadSerializable<T>() where T : new()
        {
            var result = new T();
            if (result is ISerializable serializable)
                serializable.Deserialize(this);
            return result;
        }

        public DateTime ReadDateTime() => new(ReadLong());
        
        public Type ReadType() => Type.GetType(ReadString());

        public (TKey, TValue) ReadKeyValuePair<TKey, TValue>() => (ReadWithReflection<TKey>(), ReadWithReflection<TValue>());

        public SerializationStream ReadContainer()
        {
            var bytes = ReadUnmanagedArray<byte>();
            return new SerializationStream(bytes);
        }
    }
}
#endif