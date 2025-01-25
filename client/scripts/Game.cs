using System;
using Client.scripts.Extensions;
using Client.scripts.Network;
using Core.Network.Packet;
using Core.Network.Packet.Client;
using Godot;
using LiteNetLib;
using ClientNetwork = Client.scripts.Singletons.ClientNetwork;

namespace Client.scripts;

public partial class Game : Node2D
{
    private LineEdit _idLineEdit;
    private LineEdit _nameLineEdit;
    private LineEdit _messageLineEdit;
    private Label _pingLabel;
    
    private Node2D _playersNode;
    
    private ClientNetwork _clientNetwork;
    
    public override void _Ready()
    {
        _pingLabel = GetNode<Label>("PingLabel");
        _playersNode = GetNode<Node2D>("Players");
        
        // Get Singleton ClientNetwork
        _clientNetwork = this.GetSingleton<ClientNetwork>();

        _clientNetwork.Connect(ClientNetwork.SignalName.NetworkLatencyUpdate, Callable.From<int>(UpdatePingLabel));
    }
    
    private void UpdatePingLabel(int latency)
    {
        _pingLabel.Text = $"Ping: {latency}ms";
    }
}