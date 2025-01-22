using System;
using Core.Network.Packet;
using Godot;
using LiteNetLib;

namespace Client.scripts.Network;

public class ReceiveServerPacket
{
    public void Process(PacketServerToClient packet, NetPeer peer)
    {
        foreach (var player in packet.PlayerDataList)
        {
            GD.Print($"Client: Recebido pacote ID:{player.Id}");
            GD.Print($"Client: Recebido pacote Name:{player.Name}");
        }
        
        GD.Print($"Client: Recebido pacote Message:{packet.Message}");
    }
}