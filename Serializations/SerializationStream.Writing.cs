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
            var size = sizeof(T);
            var buffer = (Span<byte>)stackalloc byte[size];
            MemoryMarshal.Write(buffer, ref v);
            Writer.Write(buffer);
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
        
        public void WriteChar(char v) => Writer.Write(v);
        
        public void WriteFloat(float v) => Writer.Write(v);
        
        public void WriteDouble(double v) => Writer.Write(v);
        
        public void WriteDecimal(decimal v) => Writer.Write(v);
        
        public void WriteString(string v) => Writer.Write(v);

        public void WriteUnmanagedIEnumerable<T>(IEnumerable<T> enumerable, int count)
        {
            var sizeOfElement = sizeof(T);
            var size = count * sizeOfElement + 4;
            var buffer = stackalloc byte[size];
            Unsafe.Copy(buffer, ref count);
            buffer += 4;
            foreach (var value in enumerable)
            {
                Unsafe.Copy(buffer, ref Unsafe.AsRef(in value));
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

        public void WriteArray<T>(T[] array) => WriteIEnumerable(array, array.Length);

        public void WriteList<T>(IList<T> list) => WriteIEnumerable(list, list.Count);

        public void WriteDictionary<T1, T2>(IDictionary<T1, T2> dictionary)
        {
            WriteIEnumerable(dictionary.Keys, dictionary.Count);
            WriteIEnumerable(dictionary.Values, dictionary.Count);
        }

        public void WriteSerializable(ISerializable serializable) => 
            serializable.Serialize(this);

        public void WriteDateTime(DateTime dateTime) =>
            WriteLong(dateTime.Ticks);

        public void WriteKeyValuePair<TKey, TValue>(TKey key, TValue value)
        {
            Write(key);
            Write(value);
        }
    }
}