using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Helpers;

public static class BuildTimeLogger
{
    private static readonly ILogger Logger;

    static BuildTimeLogger()
    {
#if DEBUG
        const LogLevel logLevel = LogLevel.Trace;
#else
        const LogLevel logLevel = LogLevel.Information;
#endif
        var loggerFactory = LoggerFactory.Create(builder => builder
            .AddSimpleConsole()
            .SetMinimumLevel(logLevel));
        
        Logger = loggerFactory.CreateLogger("Initialization");
    }

    public static void LogInformation(string? message, params object[] args)
    {
        Logger.LogInformation(message, args);
    }

    public static void LogDebug(string? message, params object[] args)
    {
        Logger.LogDebug(message, args);
    }
}