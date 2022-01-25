using System.Text;
using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.ConsoleUI;

public static class Dashboard
{
    //private static object _lock = new();
    //private static StringBuilder _sb = new();
    private static string _delimiter = new('-', 20);

    public static void DisplayProxyConnectionConfiguration(ProxyConnectionConfiguration config)
    {
        Console.WriteLine($"Connection configuration: ");
        Console.WriteLine($"   - Server url: {config.ServerUrl}");
        Console.WriteLine($"   - Server port: {config.ServerUrl}");
        Console.WriteLine($"   - Domain: {config.Domain}");
        Console.WriteLine($"   - Server connection limit: {config.MaxConnections}");
        Console.WriteLine(_delimiter);
    }

    public static void DisplayRequest()
    {

    }

    public static void DisplayResponse()
    {

    }
}