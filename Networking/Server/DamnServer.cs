using System.Net;
using System.Net.Sockets;
using System.Text;
using DamnLibrary.Networking.Packets;
using DamnLibrary.Serialization;

namespace DamnLibrary.Networking.Server;

public class DamnServer
{
    private Socket Socket { get; }

    private byte[] Buffer { get; } = new byte[ushort.MaxValue];

    public DamnServer(int port)
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localEndPoint = new IPEndPoint(IPAddress.Any, port);
        Socket.Bind(localEndPoint);

        Handle();
    }

    public DamnServer(string address, int port)
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        Socket.Connect(localEndPoint);
    }

    public async Task<int> SendAsync(ISerializable packet, IConvertible packetType, params IConvertible[] additionalData)
    {
        var serializationStream = new SerializationStream();
        var header = new PacketHeader(packetType.ToUInt16(null), additionalData.Cast<byte>().ToArray());
        
    }

    private async void Handle()
    {
        while (true)
        {
            var result = await Socket.ReceiveAsync(Buffer, SocketFlags.None);
            var bytes = Buffer.AsSpan(0, result).ToArray();

            Console.WriteLine(Encoding.UTF8.GetString(bytes));
            
            await Task.Delay(33);
        }
    }
}