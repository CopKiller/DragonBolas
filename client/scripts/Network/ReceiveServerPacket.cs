using System;
using System.Linq;
using Client.scripts.Singletons;
using Core.Network.Packet;
using Core.Network.Packet.Server;
using Godot;
using LiteNetLib;

namespace Client.scripts.Network;

public class ReceiveServerPacket(PlayerManager playerManager)
{
    private readonly PackedScene _playerScene = GD.Load<PackedScene>("res://scenes/Player.tscn");
    
    public void Process(SPlayerMove packet, NetPeer peer)
    {
        GD.Print($"Received player move packet from {peer.Address} with position {packet.Position}");
        
        playerManager.MovePlayer(packet.Index, packet.Position);
    }
    
    public void Process(SPlayerInGame packet, NetPeer peer)
    {
        if (playerManager.GetTree().CurrentScene is not Game game)
        {
            GD.PrintErr("Current scene is not Game");
            return;
        }

        foreach (var player in packet.Players)
        {
            if (playerManager.PlayerExists(player.Index)) continue;
            
            var playerInstance = _playerScene.Instantiate<Player>();
            
            if (player.Index == peer.RemoteId)
                playerInstance.IsLocalPlayer = true;
            
            playerInstance.Index = player.Index;
            
            game.GetNode("Players").AddChild(playerInstance);
        }
    }
}















