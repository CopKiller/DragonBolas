using System;
using Client.scripts.Extensions;
using Client.scripts.Network;
using Core.Network.Packet;
using Godot;
using LiteNetLib;

namespace Client.scripts;

public partial class Game : Node2D
{
    private LineEdit _idLineEdit;
    private LineEdit _nameLineEdit;
    private LineEdit _messageLineEdit;
    
    private ClientNetwork _clientNetwork;
    
    public override void _Ready()
    {
        _idLineEdit = GetNode<LineEdit>("%IdLineEdit");
        _nameLineEdit = GetNode<LineEdit>("%NameLineEdit");
        _messageLineEdit = GetNode<LineEdit>("%MessageLineEdit");
        
        // Get Singleton ClientNetwork
        _clientNetwork = this.GetSingleton<ClientNetwork>();
    }
    
    private void OnSendButtonPressed()
    {
        var packet = new PacketClientToServer
        {
            Id = Convert.ToInt32(_idLineEdit.Text),
            Name = _nameLineEdit.Text,
            Message = _messageLineEdit.Text
        };

        // Send message to server
        _clientNetwork.SendPacket(packet, DeliveryMethod.ReliableOrdered);
    }
}