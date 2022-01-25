using System.Text;

namespace GreenCobra.Client.Proxy.Abstract;

public class DumbProxyLogger : IProxyLogger
{
    private DumbProxyLogger() {}

    public static DumbProxyLogger Default => new();

    public void LogBinary(byte[] data)
    {
        var delimiter = new string('-', 50);

        Console.WriteLine();
        Console.WriteLine(delimiter);
        Console.WriteLine($"Data length: {data.Length}");
        Console.WriteLine(delimiter);

        var str = Encoding.UTF8.GetString(data);
        var segment = str.Split("\r\n\r\n");

        Console.WriteLine(segment[0]);
        Console.WriteLine(delimiter);
    }
}