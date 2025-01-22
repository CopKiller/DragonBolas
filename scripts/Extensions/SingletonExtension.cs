using Godot;

namespace Client.scripts.Extensions;

public static class SingletonExtension
{
    public static T GetSingleton<T>(this Node node) where T : Node
    {
        return node.GetTree().Root.GetNode<T>(typeof(T).Name);
    }
}