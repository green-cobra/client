using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace GreenCobra.Client.Logging;

public class UIConsoleFormatter : ConsoleFormatter
{
    private const string ConsoleFormatterName = "ConsoleUi";

    public UIConsoleFormatter() : base(ConsoleFormatterName)
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;

        }
        //var messageColor = GetLogLevelConsoleColors(logEntry.LogLevel);
        //var time = GetCurrentTime();

        //var builder = new StringBuilder();
        //builder.Append("[");
        //builder.Append(time);
        //builder.Append("] : (");
        //builder.Append(logEntry.EventId.Id);
        //builder.Append(" - ");
        //builder.Append(logEntry.EventId.Name);
        //builder.Append(") : ");
        //builder.AppendLine(message);

        //textWriter.WriteColoredMessage(builder.ToString(), messageColor);

        textWriter.Write(message);
    }

    private ConsoleColor GetLogLevelConsoleColors(LogLevel logLevel)
    {
        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => ConsoleColor.Gray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.DarkGreen,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            LogLevel.None => ConsoleColor.Gray,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    private TimeOnly GetCurrentTime()
    {
        // can be configured
        return TimeOnly.FromDateTime(DateTime.Now);
    }
}

public static class Ext
{
    internal const string DefaultForegroundColor = "\x1B[39m\x1B[22m"; // reset to default foreground color

    public static void WriteColoredMessage(this TextWriter textWriter, string message, ConsoleColor? foreground)
    {
        if (foreground.HasValue)
        {
            textWriter.Write(GetForegroundColorEscapeCode(foreground.Value));
        }
        textWriter.WriteLine(message);
        if (foreground.HasValue)
        {
            textWriter.Write(DefaultForegroundColor); // reset to default foreground color
        }
    }

    internal static string GetForegroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\x1B[30m",
            ConsoleColor.DarkRed => "\x1B[31m",
            ConsoleColor.DarkGreen => "\x1B[32m",
            ConsoleColor.DarkYellow => "\x1B[33m",
            ConsoleColor.DarkBlue => "\x1B[34m",
            ConsoleColor.DarkMagenta => "\x1B[35m",
            ConsoleColor.DarkCyan => "\x1B[36m",
            ConsoleColor.Gray => "\x1B[37m",
            ConsoleColor.Red => "\x1B[1m\x1B[31m",
            ConsoleColor.Green => "\x1B[1m\x1B[32m",
            ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
            ConsoleColor.Blue => "\x1B[1m\x1B[34m",
            ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
            ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
            ConsoleColor.White => "\x1B[1m\x1B[37m",
            //_ => DefaultForegroundColor // default foreground color
        };
    }
}
