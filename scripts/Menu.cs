using Godot;

namespace Client.scripts;

public partial class Menu : Control
{
    private PackedScene _scene;
    private Button _playbutton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playbutton = GetNode<Button>("%PlayButton");
        _playbutton.Connect(BaseButton.SignalName.Pressed,Callable.From(_playbutton_Pressed));

        _scene = GD.Load<PackedScene>("res://scenes/game.tscn");

    }

    private void _playbutton_Pressed()
    {
        GD.Print("Button Pressionado!");

        GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, _scene);
    }
}