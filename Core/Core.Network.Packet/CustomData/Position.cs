using System.Numerics;
using LiteNetLib.Utils;

namespace Core.Network.Packet.CustomData;

public struct Position(float x, float y) : IEquatable<Vector2>, INetSerializable
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;

    public bool Equals(Vector2 other)
    {
        return Math.Abs(X - other.X) < 0.01 && Math.Abs(Y - other.Y) < 0.01;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(X);
        writer.Put(Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        X = reader.GetFloat();
        Y = reader.GetFloat();
    }
}