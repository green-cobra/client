namespace GreenCobra.Client.Logging.States;

// to be able to use JsonConsoleFormatter new to override .ToString()
public interface IState
{
    public LoggingEventId EventId { get; set; }
}