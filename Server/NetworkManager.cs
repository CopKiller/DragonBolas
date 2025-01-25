using System.Diagnostics;
using Core.Network.Packet;
using Core.Network.Packet.Client;
using Core.Network.Packet.CustomData;
using Core.Network.Packet.Server;
using LiteNetLib;
using LiteNetLib.Utils;
using Server.Packet;

namespace Server;

public class NetworkManager
{
    public event Action<int>? NetworkLatencyUpdate;
    
    private readonly NetManager _netManager;
    private readonly EventBasedNetListener _networkEvent;
    private readonly NetPacketProcessor _packetProcessor;
    private readonly ReceiveClientPacket _receiveClientPacket;

    private Thread? _networkThread;
    private bool _isUpdateRunning;
    
    private readonly List<NetPeer> _peers = [];
    
    public NetworkManager()
    {
        // Register network events
        _networkEvent = new EventBasedNetListener();
        // Register network manager
        _netManager = new NetManager(_networkEvent);
        // Register packet processor
        _packetProcessor = new NetPacketProcessor();
        // Register server packet receiver
        _receiveClientPacket = new ReceiveClientPacket(_packetProcessor, _netManager);
    }

    public void Register()
    {
        RegisterEvents();
        RegisterCustomTypes();
        RegisterPackets();
    }
    
    private void RegisterCustomTypes()
    {
        _packetProcessor.RegisterNestedType<PlayerData>(() => new PlayerData());
        _packetProcessor.RegisterNestedType<Position>();
    }

    private void RegisterPackets()
    {
        
        _packetProcessor.SubscribeReusable<CPlayerMove, NetPeer>( (packet, peer) =>
        {
            _receiveClientPacket.Process(packet, peer);
        });
    }
    
    private void RegisterEvents()
    {
        _networkEvent.NetworkLatencyUpdateEvent += NetworkEvent_LatencyUpdate;
        _networkEvent.NetworkReceiveEvent += NetworkEvent_DataReceive;
        _networkEvent.ConnectionRequestEvent += NetworkEvent_ConnectionRequest;
        _networkEvent.PeerConnectedEvent += NetworkEvent_ConnectionAccept;
        _networkEvent.PeerDisconnectedEvent += NetworkEvent_PeerDisconnected;
    }
    
    public void StartNetwork(int port)
    {
        _netManager.Start(port);
        
        _isUpdateRunning = true;
        
        _networkThread = new Thread(() =>
        {
            while (_isUpdateRunning)
            {
                _netManager.PollEvents();
                Thread.Sleep(15);
            }
        });
        
        _networkThread.Start();
    }

    private void NetworkEvent_ConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("kakaka");
    }
    
    private void NetworkEvent_PeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Console.WriteLine($"Client disconnected: {peer.Address}");
    }
    
    private void NetworkEvent_ConnectionAccept(NetPeer peer)
    {
        Console.WriteLine($"Client connected: {peer.Address}");
        
        _netManager.GetPeersNonAlloc(_peers, ConnectionState.Connected);

        var inGamePacket = new SPlayerInGame();
        
        inGamePacket.Players.AddRange(_peers.Select(newPeer => new PlayerData
        {
            Index = newPeer.Id
        }));
        
        var writer = new NetDataWriter();
        _packetProcessor.Write(writer, inGamePacket);
        _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }

    private void NetworkEvent_LatencyUpdate(NetPeer peer, int latency)
    {
        NetworkLatencyUpdate?.Invoke(latency);
    }
    
    private void NetworkEvent_DataReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _packetProcessor.ReadAllPackets(reader, peer);
    }
}