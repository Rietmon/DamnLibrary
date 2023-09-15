using System.Net;
using System.Net.Sockets;
using System.Text;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Client;

public class DamnClient
{
    private Socket Socket { get; }

    private byte[] Buffer { get; } = new byte[ushort.MaxValue];

    public DamnClient(string address, int port)
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        Socket.Connect(localEndPoint);
    }

    public async Task<int> SendAsync(ISerializable packet, IConvertible packetType, params IConvertible[] additionalData)
    {
        var serializationStream = new SerializationStream();
        var header = new PacketHeader(1, packetType.ToUInt16(null), additionalData.Cast<byte>().ToArray());
        
    }
}