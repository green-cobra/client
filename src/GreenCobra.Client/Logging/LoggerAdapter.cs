using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Logging;

// todo: maybe should use ILogger<>
public class LoggerAdapter<TLoggerCategory, TState> : ILoggerAdapter<TLoggerCategory, TState>
{
    private readonly ILogger<TLoggerCategory> _logger;
    private readonly ILoggerFormatter<TState> _formatter;

    public LoggerAdapter(ILogger<TLoggerCategory> logger, ILoggerFormatter<TState> formatter)
    {
        _logger = logger;
        _formatter = formatter;
    }

    public void LogInformation(EventIds eventId, TState state, Exception? exception = null)
    {
        Log(LogLevel.Information, eventId, state, exception);
    }

    public void Log(LogLevel logLevel, EventIds eventId, TState state, Exception? exception)
    {
        if (!_logger.IsEnabled(logLevel))
            return;

        var loggerEventId = new EventId((int) eventId, eventId.ToString());
        //LoggerMessage.De
        //var message = 
        _logger.Log(logLevel, loggerEventId, state, exception,
            (s, ex) => _formatter.FormatOutput(s, ex));
    }
}

public interface ILoggerAdapter<TLoggerCategory, in TState>
{
    void LogInformation(EventIds eventId, TState state, Exception? exception = null);
}