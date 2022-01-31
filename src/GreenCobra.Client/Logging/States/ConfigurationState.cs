using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Logging.States;

public class ConfigurationState : IState, IStateFormatter<ConfigurationState>
{
    public LoggingEventId EventId { get; set; }
    public ProxyConfiguration ProxyConfiguration { get; set; }
    public ProxyServerConfiguration ConnectionConfiguration { get; set; }

    public Func<ConfigurationState, Exception?, string>? Formatter { get; } = (state, exception) =>
    {
        return $"Proxy server configuration:\r\n" +
               $"Maximum connections: {state.ConnectionConfiguration.MaxConnections}\r\n" +
               $"Server URL: {state.ConnectionConfiguration.ServerUrl}\r\n" +
               $"Server domain: {state.ConnectionConfiguration.Domain}\r\n" +
               $"Server port: {state.ConnectionConfiguration.ServerPort}\r\n" +
               $"\r\n";
    };
}