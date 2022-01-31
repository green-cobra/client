using System.Net;

namespace GreenCobra.Client.Logging.States;

public class ProxyStreamState : IState, IStateFormatter<ProxyStreamState>
{
    public LoggingEventId EventId { get; set; }
    public EndPoint From { get; set; }
    public EndPoint To { get; set; }
    public byte[] Data { get; set; }
    public int Length => Data.Length;

    public int TaskId { get; set; }

    public Func<ProxyStreamState, Exception?, string>? Formatter { get; } = (state, exception) =>
        $"{{Request ID: {state.TaskId, 10}}} " +
        $"Proxied: {state.From} =====> {state.To} " +
        $"{{{state.Length} bytes}}\r\n";
}