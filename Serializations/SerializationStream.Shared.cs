using System.Collections.Generic;
using System.IO;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private Stack<BinaryWriter> Containers { get; } = new();

        public SerializationStream() => Writer = new BinaryWriter(new MemoryStream());

        public SerializationStream(byte[] bytes) => Reader = new BinaryReader(new MemoryStream(bytes));

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
    }
}