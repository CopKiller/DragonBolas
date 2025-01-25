using LiteNetLib.Utils;

namespace Core.Network.Packet.CustomData;

public class PlayerData : INetSerializable
{
    public int Index { get; set; }
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Index);
    }

    public void Deserialize(NetDataReader reader)
    {
        Index = reader.GetInt();
    }
}