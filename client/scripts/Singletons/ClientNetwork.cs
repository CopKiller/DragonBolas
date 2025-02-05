﻿using System.Threading;
using Client.scripts.Extensions;
using Client.scripts.Network;
using Core.Network.Packet.Client;
using Core.Network.Packet.CustomData;
using Core.Network.Packet.Server;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Client.scripts.Singletons;
public partial class ClientNetwork : Node
{
    [Signal]
    public delegate void NetworkLatencyUpdateEventHandler(int latency);
    
    private NetManager _netManager;
    private EventBasedNetListener _networkEvent;
    private NetPacketProcessor _packetProcessor;
    private ReceiveServerPacket _serverPacketReceiver;
    
    private NetPeer _serverPeer;
    
    private ulong _networkTimerUpdate = 0;
    public override void _Ready()
    {
        // Register network events
        _networkEvent = new EventBasedNetListener();
        // Register network manager
        _netManager = new NetManager(_networkEvent);
        // Register packet processor
        _packetProcessor = new NetPacketProcessor();
        // Register server packet receiver
        _serverPacketReceiver = new ReceiveServerPacket(this.GetSingleton<PlayerManager>());
        
        RegisterEvents();
        
        RegisterCustomTypes();
        
        RegisterPackets();

        StartNetwork();
    }

    public void ConnectToServer()
    {
        ConnectToServer("localhost", 9050, "kakaka");
    }

    private void RegisterPackets()
    {
        _packetProcessor.SubscribeReusable<SPlayerInGame, NetPeer>( (packet, peer) =>
        {
            _serverPacketReceiver.Process(packet, peer);
        });
        _packetProcessor.SubscribeReusable<SPlayerMove, NetPeer>( (packet, peer) =>
        {
            _serverPacketReceiver.Process(packet, peer);
        });
    }

    private void RegisterCustomTypes()
    {
        _packetProcessor.RegisterNestedType<PlayerData>(() => new PlayerData());
        _packetProcessor.RegisterNestedType<Position>();
    }
    
    private void RegisterEvents()
    {
        _networkEvent.NetworkLatencyUpdateEvent += NetworkEvent_LatencyUpdate;
        _networkEvent.NetworkReceiveEvent += NetworkEvent_DataReceive;
    }
    
    private void StartNetwork()
    {
        _netManager.Start();
    }
    
    private void ConnectToServer(string ip, int port, string key)
    {
        _serverPeer = _netManager.Connect(ip, port, key);
        
        if (_serverPeer == null)
        {
            GD.PrintErr("Failed to connect to server");
        }
    }

    public override void _Process(double delta)
    {
        if (_networkTimerUpdate >= Time.GetTicksMsec()) return;
        
        _networkTimerUpdate = Time.GetTicksMsec() + 15;
        _netManager.PollEvents();
    }
    
    public void SendPacket<TPacket>(TPacket packet, DeliveryMethod deliveryMethod) where TPacket : class, new()
    {
        var writer = new NetDataWriter();
        _packetProcessor.Write(writer, packet);
        _serverPeer.Send(writer, deliveryMethod);
    }

    private void NetworkEvent_LatencyUpdate(NetPeer peer, int latency)
    {
        EmitSignal(SignalName.NetworkLatencyUpdate, latency);
    }
    
    private void NetworkEvent_DataReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _packetProcessor.ReadAllPackets(reader, peer);
    }
    
    public override void _ExitTree()
    {
        _serverPeer?.Disconnect();
    }
}
