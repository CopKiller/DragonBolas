using Core.Network.Packet.CustomData;

namespace Core.Network.Packet.Server;

public class SPlayerInGame
{
    public List<PlayerData> Players { get; set; } = [];
}