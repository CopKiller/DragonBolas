using Godot;

namespace Client.scripts;

public partial class Player : CharacterBody2D
{
    private float _speed = 200.0f;
    private AnimatedSprite2D _sprite;
    private bool _isMoving;

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        var direction = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down");

        if (direction == Vector2.Zero) return;
        
        if (Velocity == Vector2.Zero)
        {
            _isMoving = false;
        }
        if (_isMoving) return;

        Velocity = direction * _speed;

        if (direction.X != 0 && direction.Y == 0)
        {
            _sprite.Play(direction.X < 0 ? "dir_left" : "dir_right");
        }
        else
        {
            _sprite.Play(direction.Y < 0 ? "dir_up" : "dir_down");
        }
        MoveAndSlide();
    }

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
}