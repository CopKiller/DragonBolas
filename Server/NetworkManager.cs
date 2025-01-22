using System.Diagnostics;
using Core.Network.Packet;
using Core.Network.Packet.CustomData;
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
    
    public NetworkManager()
    {
        // Register network events
        _networkEvent = new EventBasedNetListener();
        // Register network manager
        _netManager = new NetManager(_networkEvent);
        // Register packet processor
        _packetProcessor = new NetPacketProcessor();
        // Register server packet receiver
        _receiveClientPacket = new ReceiveClientPacket();
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
    }

    private void RegisterPackets()
    {
        _packetProcessor.SubscribeReusable<PacketClientToServer, NetPeer>( (packet, peer) =>
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
    
    private void NetworkEvent_ConnectionAccept(NetPeer peer)
    {
        Console.WriteLine($"Client connected: {peer.Address}");
        
        
        PacketServerToClient packet = new()
        {
            PlayerDataList =
            [
                new PlayerData()
                {
                    Id = 1,
                    Name = "Player 1"
                },

                new PlayerData()
                {
                    Id = 2,
                    Name = "Player 2",
                },

                new PlayerData()
                {
                    Id = 3,
                    Name = "Player 3",
                }
            ]
        };
        
        var writer = new NetDataWriter();
        _packetProcessor.Write(writer, packet);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
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