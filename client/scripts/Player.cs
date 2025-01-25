using Client.scripts.Extensions;
using Client.scripts.Singletons;
using Core.Network.Packet.Client;
using Core.Network.Packet.CustomData;
using Godot;
using LiteNetLib;

namespace Client.scripts;

public partial class Player : CharacterBody2D
{
    private float _speed = 200.0f;
    private AnimatedSprite2D _sprite;
    private Vector2 _direction = Vector2.Zero;
    
    private bool _isMoving;
    
    public bool IsLocalPlayer;
    
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        _clientNetwork = this.GetSingleton<ClientNetwork>();
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleInputMovement(delta);

        HandlePlayerAnimation();
        
        MoveAndSlide();
    }
    
    private void HandleInputMovement(double delta)
    {
        if (!IsLocalPlayer) return;
        
        if (_isMoving) return;
        
        _direction = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down");

        if (_direction == Vector2.Zero)
        {
            Velocity = Vector2.Zero;
            return;
        }

        Velocity = (_direction * _speed);
        
        SendPlayerMove();
    }
    
    private void HandlePlayerAnimation()
    {
        if (_direction.X != 0 && _direction.Y == 0)
            _sprite.Play(_direction.X < 0 ? "dir_left" : "dir_right");
        else
            _sprite.Play(_direction.Y < 0 ? "dir_up" : "dir_down");
    }
    
    
    # region NetworkLayer
    private ClientNetwork _clientNetwork;
    
    public int Index { get; set; }
    
    private void SendPlayerMove()
    {
        var packet = new CPlayerMove
        {
            Position = new Position
            {
                X = Position.X,
                Y = Position.Y
            }
        };
        
        _clientNetwork.SendPacket(packet, DeliveryMethod.ReliableOrdered);
    }
    #endregion
}