#if ENABLE_SERIALIZATION
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DamnLibrary.Utilities;
using DamnLibrary.Utilities.Extensions;
#if UNITY_5_3_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#else
using System.Runtime.CompilerServices;
#endif

namespace DamnLibrary.Serializations
{
    public unsafe partial class SerializationStream
    {
        public Stream BaseStream => Writer?.BaseStream ?? Reader.BaseStream;
        
        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }
        
        public long Length => BaseStream.Length;
        
        public bool HasToRead => Reader.BaseStream.Length > Reader.BaseStream.Position;
        
        public bool IsEmpty => Length == 0;
        
        public bool IsWriting => Writer != null;
        
        public bool IsReading => Reader != null;
        
        private Stack<BinaryWriter> Containers { get; } = new();

        public SerializationStream() => 
            Writer = new BinaryWriter(new MemoryStream());

        public SerializationStream(byte[] bytes, string encryptionKey = "") => 
            Reader = new BinaryReader(new MemoryStream(
                encryptionKey.IsNullOrEmpty() 
                    ? bytes 
                    : bytes.Decrypt(encryptionKey)));

        public void BeginContainer()
        {
            var stream = new BinaryWriter(new MemoryStream());
            if (Writer != null)
                Containers.Push(Writer);
            Writer = stream;
        }

        public void EndContainer()
        {
            if (Containers.Count <= 0)
                return;

            var oldStream = Writer;
            var bytes = ((MemoryStream)oldStream.BaseStream).ToArray();
            Writer = Containers.Pop();
            WriteUnmanagedIEnumerable(bytes, bytes.Length);
            oldStream.Dispose();
        }

        public byte[] ToArray()
        {
            var savedPosition = Writer.BaseStream.Position;
            Writer.BaseStream.Position = 0;
            var bytes = new byte[Writer.BaseStream.Length];
            var read = Writer.BaseStream.Read(bytes, 0, bytes.Length);
            Writer.BaseStream.Position = savedPosition;
            return read == bytes.Length ? bytes : null;
        }

        public byte[] ToEncryptedArray(string key)
        {
            var bytes = ToArray();

            return bytes?.Encrypt(key);
        }

        private int Unsafe_SizeOf<T>() =>
#if UNITY_5_3_OR_NEWER
            Marshal.SizeOf<T>();
#else
            Unsafe.SizeOf<T>();
#endif

#if !UNITY_5_3_OR_NEWER
        private T Unsafe_ReadNetCore<T>(void* buffer, int index, int sizeOfElement) =>
            Unsafe.Read<T>((byte*)buffer + index * sizeOfElement);
#else
        private T Unsafe_ReadUnity<T>(void* buffer, int index) => 
            UnsafeUtility.ReadArrayElement<T>(buffer, index);
#endif

        private void Unsafe_MemoryCopy<T>(void* buffer, ref T value) =>
#if !UNITY_5_3_OR_NEWER
            Unsafe.Copy(buffer, ref value);
#else
            UnsafeUtility.WriteArrayElement(buffer, 0, value);
#endif

        private ref T Unsafe_AsRef<T>(ref T value) =>
#if !UNITY_5_3_OR_NEWER
            ref Unsafe.AsRef(in value);
#else
            ref UnsafeUtility.As<T, T>(ref value);

#endif

    }
}
#endif