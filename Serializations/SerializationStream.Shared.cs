using System.Collections.Generic;
using System.IO;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private Stream Stream { get; set; }
        
        private Stack<Stream> Containers { get; } = new();
        
        private byte[] Buffer { get; } = new byte[1024];
        
        public SerializationStream() => Stream = new MemoryStream();

        public SerializationStream(Stream currentStream) => Stream = currentStream;

        public SerializationStream(byte[] bytes) => Stream = new MemoryStream(bytes);

        public void BeginContainer()
        {
            var stream = new MemoryStream();
            if (Stream != null)
                Containers.Push(Stream);
            Stream = stream;
        }

        public void EndContainer()
        {
            if (Containers.Count <= 0)
                return;

            var oldStream = (MemoryStream)Stream;
            var bytes = oldStream.ToArray();
            Stream = Containers.Pop();
            WriteUnmanagedIEnumerable(bytes, bytes.Length);
            oldStream.Dispose();
        }

        public byte[] ToArray()
        {
            var savedPosition = Stream.Position;
            Stream.Position = 0;
            var bytes = new byte[Stream.Length];
            var read = Stream.Read(bytes, 0, bytes.Length);
            Stream.Position = savedPosition;
            return read == bytes.Length ? bytes : null;
        }
    }
}