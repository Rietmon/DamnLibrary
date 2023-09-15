using System.Net;
using System.Net.Sockets;
using System.Text;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Networking.Packets.Handles;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Protocols.Udp;

public class UdpSocket
{
    private Socket Socket { get; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private byte[] Buffer { get; } = new byte[0x1000];

    public void Listen(int port)
    {
        var endPoint = new IPEndPoint(IPAddress.Any, port);
        Socket.Bind(endPoint);
    }

    public void Connect(string ipAddress, ushort port)
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        Socket.Connect(endPoint);
    }
    
    private async void Handle()
    {
        while (true)
        {
            var result = await Socket.ReceiveAsync(Buffer, SocketFlags.None);
            var bytes = Buffer.AsSpan(0, result).ToArray();

            var startPosition = 0;
            while (GetNextPacket(bytes, startPosition, out startPosition) is { } packetBytes)
                HandlePacket(packetBytes);
            
            await Task.Delay(33);
        }
    }

    private static void HandlePacket(byte[] packetBytes)
    {
        var stream = new SerializationStream(packetBytes);
        var header = stream.Read<PacketHeader>();

        PacketHandler.HandlePacket(header, stream);
    }

    private static unsafe byte[] GetNextPacket(byte[] buffer, int startPosition, out int newStartPosition)
    {
        newStartPosition = -1;
        var bufferLength = buffer.Length;
        fixed (byte* pBuffer = buffer)
        {
            for (var i = startPosition; i < bufferLength; i++)
            {
                var endOfPacketMark = *(ulong*)(pBuffer + i);
                if (endOfPacketMark == 300849)
                {
                    var packetBytes = buffer.AsSpan(startPosition, i).ToArray();
                    newStartPosition = i + sizeof(ulong);
                    return packetBytes;
                }
            }
        }

        return null;
    }
}