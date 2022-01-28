using System.Net;

namespace GreenCobra.Client.Logging.States;

public class ProxyStreamState : IState, IStateFormatter<ProxyStreamState>
{
    public LoggingEventId EventId { get; set; }
    public EndPoint From { get; set; }
    public EndPoint To { get; set; }
    public byte[] Data { get; set; }
    public int Length => Data.Length;

    public Func<ProxyStreamState, Exception?, string>? Formatter { get; } = (state, exception) =>
    {
        return $"{state.From} ===> {state.To}:\r\nProxied {state.Length}.";
    };
}