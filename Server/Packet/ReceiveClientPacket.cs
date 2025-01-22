using Core.Network.Packet;
using LiteNetLib;

namespace Server.Packet;

public class ReceiveClientPacket
{
    public void Process(PacketClientToServer packet, NetPeer peer)
    {
        Console.WriteLine($"Server: Recebido pacote ID:{packet.Id}");
        Console.WriteLine($"Server: Recebido pacote Name:{packet.Name}");
        Console.WriteLine($"Server: Recebido pacote Message:{packet.Message}");
    }
}