using GreenCobra.Client.Logging.States.Interfaces;

namespace GreenCobra.Client.Logging.Adapters;

public interface ILoggerAdapter<TLoggerCategory>
{
    void LogInformation<TState>(TState state) where TState : IState, IStateFormatter<TState>;
}