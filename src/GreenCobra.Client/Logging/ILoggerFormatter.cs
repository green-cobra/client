using System.Text;

namespace GreenCobra.Client.Logging;

public class StringLoggerFormatter : ILoggerFormatter<string>
{
    public string FormatOutput(string input, Exception? ex)
    {
        // todo: check Exception
        return input;
    }
}

public class BinaryLoggerFormatter : ILoggerFormatter<byte[]>
{
    public string FormatOutput(byte[] input, Exception? ex)
    {
        //Console.WriteLine();
        //Console.WriteLine(delimiter);
        var dataLength = $"Data length: {input.Length}";
        //Console.WriteLine(delimiter);

        var str = Encoding.UTF8.GetString(input);
        var segment = str.Split("\r\n\r\n");

        Console.WriteLine(segment[0]);
        //Console.WriteLine(delimiter);

        return $"{dataLength}\r\n{str}";
    }
}

public interface ILoggerFormatter<in TState>
{
    string FormatOutput(TState state, Exception? ex);
}

//public class LoggerMessage<T> where T : IStateInfo
//{
//    public TimeOnly Time { get; } = TimeOnly.FromDateTime(DateTime.Now);
//    public EventId EventId { get; set; }
//    //public string Category { get; set; } = string.Empty;
//    public string Message { get; set; } = string.Empty;
//    public LogLevel LogLevel { get; set; }
//    public T State { get; set; }
//}

public enum EventIds
{
    // config events
    RetrievedProxyPoolConfig = 10,
    RetrievedProxyServerConfig = 20,

    // proxying events
    ProxyTaskCompleted = 100,
    DataProxied = 1000,

    // other events
    PoolWatcher_GotPoolStatus = 5000
}