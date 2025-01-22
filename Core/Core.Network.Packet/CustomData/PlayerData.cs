using LiteNetLib.Utils;

namespace Core.Network.Packet.CustomData;

public class PlayerData : INetSerializable
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(Message);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        Name = reader.GetString();
        Message = reader.GetString();
    }
}