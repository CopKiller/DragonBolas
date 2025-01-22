using Core.Network.Packet.CustomData;

namespace Core.Network.Packet;

public class PacketServerToClient
{
    public List<PlayerData> PlayerDataList { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}