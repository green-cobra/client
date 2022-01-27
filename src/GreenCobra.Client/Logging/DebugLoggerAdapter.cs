using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Logging;

public class BaseLoggerAdapter<TLogger, TState> : ILoggerAdapter<TLogger, TState>
{
    private readonly ILogger<TLogger> _logger;
    private readonly ILoggerFormatter<TState> _formatter;

    public BaseLoggerAdapter(ILogger<TLogger> logger, ILoggerFormatter<TState> formatter)
    {
        _logger = logger;
        _formatter = formatter;
    }

    public void LogInfo(TState message)
    {
        //var message = 
        _logger.Log(LogLevel.Information, 0, message, null,
            (state, exception) => _formatter.FormatOutput(state));
    }
}

public interface ILoggerAdapter<TLogger, in TState>
{
    void LogInfo(TState state);
}