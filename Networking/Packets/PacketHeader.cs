#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Packets
{
    public struct PacketHeader : ISerializable
    {
        public static Type PacketTypeSerializationType { get; set; } = typeof(byte);
        
        public uint Id { get; set; }
        
        public bool IsResponse { get; set; }
        public bool NeedResponse { get; set; }
        
        public IConvertible Type { get; set; }

        public byte[] AdditionData { get; set; }

        public void Serialize(SerializationStream stream)
        {
            stream.Write(Id);
            stream.Write(IsResponse);
            stream.Write(NeedResponse);
            stream.Write(Type);
            stream.Write(AdditionData);
        }

        public void Deserialize(SerializationStream stream)
        {
            Id = stream.Read<uint>();
            IsResponse = stream.Read<bool>();
            NeedResponse = stream.Read<bool>();
            Type = (IConvertible)stream.Read(PacketTypeSerializationType);
            AdditionData = stream.Read<byte[]>();
        }
    }
}
#endif