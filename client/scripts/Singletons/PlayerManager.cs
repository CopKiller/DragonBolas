using System.Collections.Generic;
using Core.Network.Packet.CustomData;
using Godot;

namespace Client.scripts.Singletons;

public partial class PlayerManager : Node
{
    private Dictionary<int, Player> _players = new();

    public override void _Ready()
    {
        GetTree().Connect(SceneTree.SignalName.NodeAdded, Callable.From<Node>(NodeAdded));
    }

    public Player GetPlayer(int index)
    {
        if (_players.TryGetValue(index, out var player)) return _players[index];
        
        GD.PrintErr("Player not found");
        return null;
    }
    
    public bool PlayerExists(int index)
    {
        return _players.ContainsKey(index);
    }
    
    public void MovePlayer(int index, Position position)
    {
        var player = GetPlayer(index);
        if (player == null) return;
        
        player.Position = new Vector2(position.X, position.Y);
    }
    
    private void NodeAdded(Node node)
    {
        if (node is not Player player) return;
        
        _players.Add(player.Index, player);
        
        player.Connect(Node.SignalName.TreeExited, Callable.From<Node>((nod) => RemovePlayerFromDictionary(player.Index)));
    }
    
    private void RemovePlayerFromDictionary(int index)
    {
        _players.Remove(index);
    }
}