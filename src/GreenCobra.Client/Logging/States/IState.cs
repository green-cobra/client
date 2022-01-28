namespace GreenCobra.Client.Logging.States;

public interface IState
{
    public LoggingEventId EventId { get; set; }
}