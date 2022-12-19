using System;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public struct PacketHeader : ISerializable
    {
        public uint Id { get; set; }
        
        public bool IsResponse { get; set; }
        
        public IConvertible Type { get; set; }

        public byte[] AdditionData { get; set; }

        public void Serialize(SerializationStream stream)
        {
            stream.Write(Id);
            stream.Write(IsResponse);
            stream.Write(Type);
            stream.Write(AdditionData);
        }

        public void Deserialize(SerializationStream stream)
        {
            Id = stream.Read<uint>();
            IsResponse = stream.Read<bool>();
            Type = stream.Read<IConvertible>();
            AdditionData = stream.Read<byte[]>();
        }
    }
}