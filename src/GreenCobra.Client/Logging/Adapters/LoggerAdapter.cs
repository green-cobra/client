using GreenCobra.Client.Logging.States.Interfaces;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Logging.Adapters;

public class LoggerAdapter<TLoggerCategory> : ILoggerAdapter<TLoggerCategory>
{
    private readonly ILogger<TLoggerCategory> _logger;

    public LoggerAdapter(ILogger<TLoggerCategory> logger)
    {
        _logger = logger;
    }

    public void LogInformation<TState>(TState state)
        where TState : IState, IStateFormatter<TState>
    {
        if (!_logger.IsEnabled(LogLevel.Information)) return;

        var logEvent = new EventId((int)state.EventId, state.EventId.ToString());
        
        _logger.Log(LogLevel.Information, logEvent, state, null, state.Formatter);
    }
}