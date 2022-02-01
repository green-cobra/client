using GreenCobra.Client.Logging.States.Interfaces;

namespace GreenCobra.Client.Logging.States;

public record SimpleMessageState(LoggingEventId EventId, string Message)
    : IState, IStateFormatter<SimpleMessageState>
{
    public Func<SimpleMessageState, Exception?, string> Formatter { get; } = (state, ex) => state.Message;
}