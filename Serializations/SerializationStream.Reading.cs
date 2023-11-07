using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public unsafe partial class SerializationStream
    {
        public bool HasToRead => Stream.Length > Stream.Position;

        public T ReadUnmanaged<T>() where T : unmanaged
        {
            if (!ReadBytesToBuffer(sizeof(T)))
                return default;
            
            fixed (byte* arrayPtr = Buffer)
                return *(T*)arrayPtr;
        }
        
        public bool ReadBool()
        {
            if (!ReadBytesToBuffer(1))
                return default;

            return Buffer[0] == 1;
        }
        
        public sbyte ReadSByte()
        {
            if (!ReadBytesToBuffer(1))
                return default;

            return (sbyte)(Buffer[0] - 128);
        }
        
        public byte ReadByte()
        {
            if (!ReadBytesToBuffer(1))
                return default;

            return Buffer[0];
        }
        
        public short ReadShort()
        {
            if (!ReadBytesToBuffer(2))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(short*)arrayPtr;
        }
        
        public ushort ReadUShort()
        {
            if (!ReadBytesToBuffer(2))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(ushort*)arrayPtr;
        }
        
        public int ReadInt()
        {
            if (!ReadBytesToBuffer(4))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(int*)arrayPtr;
        }
        
        public uint ReadUInt()
        {
            if (!ReadBytesToBuffer(4))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(uint*)arrayPtr;
        }
        
        public long ReadLong()
        {
            if (!ReadBytesToBuffer(8))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(long*)arrayPtr;
        }
        
        public ulong ReadULong()
        {
            if (!ReadBytesToBuffer(8))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(ulong*)arrayPtr;
        }
        
        public char ReadChar()
        {
            if (!ReadBytesToBuffer(2))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(char*)arrayPtr;
        }
        
        public float ReadFloat()
        {
            if (!ReadBytesToBuffer(4))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(float*)arrayPtr;
        }
        
        public double ReadDouble()
        {
            if (!ReadBytesToBuffer(8))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(double*)arrayPtr;
        }
        
        public string ReadString()
        {
            var length = ReadInt();
            if (!ReadBytesToBuffer(length * 2))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return Encoding.UTF8.GetString(arrayPtr, length);
        }
        
        public decimal ReadDecimal()
        {
            if (!ReadBytesToBuffer(16))
                return default;

            fixed (byte* arrayPtr = Buffer)
                return *(decimal*)arrayPtr;
        }

        public T[] ReadUnmanagedArray<T>() where T : unmanaged
        {
            var length = ReadInt();
            var size = Marshal.SizeOf<T>();
            if (!ReadBytesToBuffer(length * size))
                return default;
            
            var array = new T[length];
            fixed (byte* arrayPtr = Buffer)
            {
                for (var i = 0; i < length; i++)
                    array[i] = *(T*)(arrayPtr + i * size);
            }

            return array;
        }

        public T[] ReadManagedArray<T>()
        {
            var length = ReadInt();
            var array = new T[length];
            for (var i = 0; i < length; i++)
                array[i] = ReadWithReflection<T>();
            return array;
        }

        public List<T> ReadUnmanagedList<T>() where T : unmanaged => new(ReadUnmanagedArray<T>());

        public List<T> ReadList<T>() => new(ReadManagedArray<T>());
            
        public Dictionary<TKey, TValue> ReadUnmanagedDictionary<TKey, TValue>() 
            where TKey : unmanaged 
            where TValue : unmanaged
        {
            var result = new Dictionary<TKey, TValue>();
            var keys = ReadUnmanagedArray<TKey>();
            var values = ReadUnmanagedArray<TValue>();
            for (var i = 0; i < keys.Length; i++)
                result.Add(keys[i], values[i]);
            return result;
        }
            
        public Dictionary<TKey, TValue> ReadManagedDictionary<TKey, TValue>()
        {
            var result = new Dictionary<TKey, TValue>();
            var keys = ReadManagedArray<TKey>();
            var values = ReadManagedArray<TValue>();
            for (var i = 0; i < keys.Length; i++)
                result.Add(keys[i], values[i]);
            return result;
        }

        public T ReadSerializable<T>() where T : ISerializable, new()
        {
            var result = new T();
            result.Deserialize(this);
            return result;
        }

        public DateTime ReadDateTime() => new(ReadLong());
        
        public Type ReadType() => Type.GetType(ReadString());

        public (TKey, TValue) ReadKeyValuePair<TKey, TValue>() => (ReadWithReflection<TKey>(), ReadWithReflection<TValue>());
        public (object, object) ReadKeyValuePair(Type keyType, Type valueType) => 
            (ReadWithReflection(keyType), ReadWithReflection(valueType));

        public SerializationStream ReadContainer()
        {
            var bytes = ReadUnmanagedArray<byte>();
            return new SerializationStream(bytes);
        }
        
        private bool ReadBytesToBuffer(int size)
        {
            var read = Stream.Read(Buffer, 0, size);
            return read == size;
        }
    }
}