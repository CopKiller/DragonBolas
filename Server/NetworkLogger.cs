using LiteNetLib;

namespace Server;

public class NetworkLogger : INetLogger
{
    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Console.WriteLine(str, args);
    }

}
