namespace Core.Network.Packet;

public class PacketClientToServer
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
}