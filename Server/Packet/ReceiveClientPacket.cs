using Core.Network.Packet;
using Core.Network.Packet.Client;
using Core.Network.Packet.Server;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server.Packet;

public class ReceiveClientPacket(NetPacketProcessor packetProcessor, NetManager netManager)
{
    private readonly NetDataWriter _dataWriter = new();
    
    public void Process(CPlayerMove packet, NetPeer peer)
    {
        // Enviar o pacote de volta pra todos os jogadores sincronizar a posição deste jogador.
        _dataWriter.Reset();
        
        var serverPlayerMove = new SPlayerMove
        {
            Index = peer.Id,
            Position = packet.Position
        };
        
        packetProcessor.Write(_dataWriter, serverPlayerMove);
        
        netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered, peer);
    }
}