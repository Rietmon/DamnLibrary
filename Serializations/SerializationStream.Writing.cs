#if ENABLE_SERIALIZATION
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public unsafe partial class SerializationStream
    {
        private BinaryWriter Writer { get; set; }

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
                case float v: WriteFloat(v); return;
                case double v: WriteDouble(v); return;
                case decimal v: WriteDecimal(v); return;
                case char v: WriteChar(v); return;
                case string v: WriteString(v); return;
                case Array: throw new Exception("Use WriteArray instead of Write<T[]>"!);
                case IList: case IList<T>: throw new Exception("Use WriteList instead of Write<IList>"!);
                case IDictionary: throw new Exception("Use WriteDictionary instead of Write<IDictionary>"!);
                case ISerializable v: WriteSerializable(v); return;
                case DateTime v: WriteDateTime(v); return;
                case Type v: WriteType(v); return;
            }
        }
        
        public void WriteUnmanaged<T>(T v) where T : unmanaged
        {
            var size = sizeof(T);
            var buffer = (Span<byte>)stackalloc byte[size];
            MemoryMarshal.Write(buffer, ref v);
            Writer.Write(buffer);
        }

        public void WriteForceUnmanaged<T>(T v)
        {
            var sizeOfElement = Unsafe_SizeOf<T>();
            var buffer = stackalloc byte[sizeOfElement];
            Unsafe_MemoryCopy(buffer, ref v);
            var span = new ReadOnlySpan<byte>(buffer, sizeOfElement);
            Writer.Write(span);
        }

        public void WriteBool(bool v) => Writer.Write(v);
        
        public void WriteSByte(sbyte v) => Writer.Write(v);
        
        public void WriteByte(byte v) => Writer.Write(v);
        
        public void WriteShort(short v) => Writer.Write(v);
        
        public void WriteUShort(ushort v) => Writer.Write(v);
        
        public void WriteInt(int v) => Writer.Write(v);
        
        public void WriteUInt(uint v) => Writer.Write(v);
        
        public void WriteLong(long v) => Writer.Write(v);
        
        public void WriteULong(ulong v) => Writer.Write(v);
        
        public void WriteFloat(float v) => Writer.Write(v);
        
        public void WriteDouble(double v) => Writer.Write(v);
        
        public void WriteDecimal(decimal v) => Writer.Write(v);
        
        public void WriteChar(char v) => Writer.Write(v);
        
        public void WriteString(string v) => Writer.Write(v);

        public void WriteUnmanagedIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            var sizeOfElement = Unsafe_SizeOf<T>();
            var size = count * sizeOfElement + 4;
            var buffer = stackalloc byte[size];
            Unsafe_MemoryCopy(buffer, ref count);
            buffer += 4;
            foreach (var value in enumerable)
            {
                var v = value;
                Unsafe_MemoryCopy(buffer, ref Unsafe_AsRef(ref v));
                buffer += sizeOfElement;
            }

            var span = new ReadOnlySpan<byte>(buffer - size, size);
            Writer.Write(span);
        }

        public void WriteManagedIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                Write(value);
        }
        
        public void WriteIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                WriteManagedIEnumerable(enumerable, count);
            else 
                WriteUnmanagedIEnumerable(enumerable, count);
        }
        
        public void WriteNonGenericIEnumerable(IEnumerable enumerable, int count)
        {
            WriteInt(count);
            foreach (var value in enumerable)
                WriteBoxed(value);
        }

        public void WriteArray<T>(T[] array) => WriteIEnumerable(array, array.Length);

        public void WriteList<T>(IList<T> list) => WriteIEnumerable(list, list.Count);

        public void WriteDictionary<T1, T2>(IDictionary<T1, T2> dictionary)
        {
            var count = dictionary.Count;
            WriteIEnumerable(dictionary.Keys, count);
            WriteIEnumerable(dictionary.Values, count);
        }

        public void WriteSerializable(ISerializable serializable) => 
            serializable.Serialize(this);

        public void WriteDateTime(DateTime dateTime) =>
            WriteLong(dateTime.Ticks);

        public void WriteType(Type type) =>
            WriteString(type.AssemblyQualifiedName);

        public void WriteBoxed(object value)
        {
            WriteType(value.GetType());
            Write(value);
        }
        
        public void WriteKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> pair) =>
            WriteKeyValuePair(pair.Key, pair.Value);

        public void WriteKeyValuePair<TKey, TValue>(TKey key, TValue value)
        {
            Write(key);
            Write(value);
        }
    }
}
#endif