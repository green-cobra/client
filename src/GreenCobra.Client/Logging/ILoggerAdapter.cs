using GreenCobra.Client.Logging.States;

namespace GreenCobra.Client.Logging;

public interface ILoggerAdapter<TLoggerCategory>
{
    void LogInformation<TState>(TState state) where TState : IState, IStateFormatter<TState>;
}