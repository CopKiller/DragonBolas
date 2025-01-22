namespace Server;

internal abstract class Program
{
    private static NetworkManager? _networkManager;
    
    private static async Task Main(string[] args)
    {
        _networkManager = new NetworkManager();
        
        _networkManager.Register();
        _networkManager.StartNetwork(9050);
        
        Console.WriteLine("Server started!");
        
        await Task.Delay(-1);
    }
}