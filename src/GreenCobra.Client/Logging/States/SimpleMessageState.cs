namespace GreenCobra.Client.Logging.States;

public class SimpleMessageState : IState, IStateFormatter<SimpleMessageState>
{
    public LoggingEventId EventId { get; set; }
    public string Message { get; set; }

    public Func<SimpleMessageState, Exception?, string>? Formatter { get; } = (state, ex) => state.Message;
}