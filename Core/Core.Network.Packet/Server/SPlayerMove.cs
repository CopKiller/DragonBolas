using Core.Network.Packet.CustomData;

namespace Core.Network.Packet.Server;

public class SPlayerMove
{
    public int Index { get; set; }
    public Position Position { get; set; }
}