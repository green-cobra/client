using System.Text;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Logging;

public class DefaultLoggerFormatter : ILoggerFormatter<string>
{
    public string FormatOutput(string input)
    {
        return input;
    }
}

public class BinaryLoggerFormatter : ILoggerFormatter<byte[]>
{
    public string FormatOutput(byte[] input)
    {
        //Console.WriteLine();
        //Console.WriteLine(delimiter);
        var dataLength = $"Data length: {input.Length}";
        //Console.WriteLine(delimiter);

        var str = Encoding.UTF8.GetString(input);
        //var segment = str.Split("\r\n\r\n");

        //Console.WriteLine(segment[0]);
        //Console.WriteLine(delimiter);

        return $"{dataLength}\r\n{str}";
    }
}

public interface ILoggerFormatter<in T>
{
    string FormatOutput(T input);
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

// public record ProxyTaskState(int Id, string Name);
//public interface IStateInfo
//{
//    string GetInOutputFormat();
//}

//public enum EventId { }