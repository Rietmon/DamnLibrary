using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DamnLibrary.Serializations.Serializables;
#if UNITY_5_3_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace DamnLibrary.Serializations
{
    public unsafe partial class SerializationStream
    {
        public void Write<T>(T value)
        {
            switch (value)
            {
                case bool v: WriteBool(v); return;
                case sbyte v: WriteSByte(v); return;
                case byte v: WriteByte(v); return;
                case short v: WriteShort(v); return;
                case ushort v: WriteUShort(v); return;
                case int v: WriteInt(v); return;
                case uint v: WriteUInt(v); return;
                case long v: WriteLong(v); return;
                case ulong v: WriteULong(v); return;
                case char v: WriteChar(v); return;
                case float v: WriteFloat(v); return;
                case double v: WriteDouble(v); return;
                case string v: WriteString(v); return;
                case decimal v: WriteDecimal(v); return;
                case Array: throw new Exception("Use WriteArray instead of Write<T[]>"!);
                case IList: case IList<T>: throw new Exception("Use WriteList instead of Write<IList>"!);
                case IDictionary: throw new Exception("Use WriteDictionary instead of Write<IDictionary>"!);
                case ISerializable v: WriteSerializable(v); return;
                case DateTime v: WriteDateTime(v); return;
                case Type v: WriteString(v.FullName); return;
            }

            var type = typeof(T);
            if (TryGetSerializationActions(type, out var methods))
            {
                methods.Item1(this);
                return;
            }

            WriteWithReflection(value, type);
        }

        public void WriteUnmanaged<T>(T v) where T : unmanaged
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, sizeof(T));
        }
        
        public void WriteBool(bool v)
        {
            Buffer[0] = v ? (byte)1 : (byte)0;
            Stream.Write(Buffer, 0, 1);
        }
        
        public void WriteSByte(sbyte v)
        {
            Buffer[0] = (byte)(v + 128);
            Stream.Write(Buffer, 0, 1);
        }
        
        public void WriteByte(byte v)
        {
            Buffer[0] = v;
            Stream.Write(Buffer, 0, 1);
        }
        
        public void WriteShort(short v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 2);
        }
        
        public void WriteUShort(ushort v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 2);
        }
        
        public void WriteInt(int v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 4);
        }
        
        public void WriteUInt(uint v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 4);
        }
        
        public void WriteLong(long v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 8);
        }
        
        public void WriteULong(ulong v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 8);
        }
        
        public void WriteChar(char v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 2);
        }
        
        public void WriteFloat(float v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 4);
        }
        
        public void WriteDouble(double v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 8);
        }
        
        public void WriteString(string v)
        {
            WriteLengthToBuffer(v.Length);
            Encoding.UTF8.GetBytes(v, 0, v.Length, Buffer, 4);
            Stream.Write(Buffer, 0, v.Length * 2 + 4);
        }
        
        public void WriteDecimal(decimal v)
        {
            var valuePtr = (byte*)&v;
            WriteBytesToBufferAndStream(valuePtr, 16);
        }
        
        public void WriteIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                WriteManagedIEnumerable(enumerable, count);
            else 
                WriteUnmanagedIEnumerable(enumerable, count);
        }

        public void WriteArray<T>(T[] array) => WriteIEnumerable(array, array.Length);

        public void WriteList<T>(IList<T> list) => WriteIEnumerable(list, list.Count);

        public void WriteDictionary<T1, T2>(IDictionary<T1, T2> dictionary)
        {
            WriteIEnumerable(dictionary.Keys, dictionary.Count);
            WriteIEnumerable(dictionary.Values, dictionary.Count);
        }

        public void WriteManagedIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                Write(value);
        }

        public void WriteUnmanagedIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            var sizeOfElement = (uint)Marshal.SizeOf<T>();
            uint currentSize = 0;
            fixed (byte* arrayPtr = Buffer)
            {
                MemoryCopy(arrayPtr, &count, 4);
                currentSize += 4;
                
                foreach (var value in enumerable)
                {
                    var valuePtr = (byte*)&value;
                    MemoryCopy(arrayPtr + currentSize, valuePtr, sizeOfElement);
                    currentSize += sizeOfElement;
                }
            }
            Stream.Write(Buffer, 0, (int)currentSize);
        }

        public void WriteNonGenericIEnumerable(IEnumerable enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                WriteObject(value);
        }

        public void WriteSerializable(ISerializable serializable) => 
            serializable.Serialize(this);

        public void WriteDateTime(DateTime dateTime) =>
            WriteLong(dateTime.Ticks);

        public void WriteObject(object value)
        {
            WriteString(value.GetType().FullName);
            Write(value);
        }

        public void WriteKeyValuePair<TKey, TValue>(TKey key, TValue value)
        {
            Write(key);
            Write(value);
        }
        
        private void WriteLengthToBuffer(int length)
        {
            var lengthPtr = &length;
            fixed (byte* arrayPtr = Buffer)
                MemoryCopy(arrayPtr, lengthPtr, 4);
        }
        
        private void WriteBytesToBufferAndStream(byte* valuePtr, int size)
        {
            fixed (byte* arrayPtr = Buffer)
                MemoryCopy(arrayPtr, valuePtr, (uint)size);
            Stream.Write(Buffer, 0, size);
        }
        
        private static void MemoryCopy(void* source, void* destination, uint size) => 
#if !UNITY_5_3_OR_NEWER
            Unsafe.CopyBlock(destination, source, size);
#else
            UnsafeUtility.MemCpy(destination, source, size);
#endif
    }
}