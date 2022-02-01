using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Logging.States;

public class GotProxyConfigurationState : IState, IStateFormatter<GotProxyConfigurationState>
{
    public LoggingEventId EventId => LoggingEventId.GotServerConfiguration;
    public ProxyConfiguration ProxyConfiguration { get; set; }
    public Uri ServerUrl { get; set; }

    public Func<GotProxyConfigurationState, Exception?, string>? Formatter { get; } = (state, exception) =>
    {
        return $"Proxy server configuration:\r\n" +
               $"Server URL         : {state.ServerUrl}\r\n" +
               $"Server end point   : {state.ProxyConfiguration.ServerEndPoint}\r\n" +
               $"Client end point   : {state.ProxyConfiguration.ClientEndPoint}\r\n" +
               $"Maximum connections: {state.ProxyConfiguration.MaxConnections}\r\n" +
               $"\r\n";
    };
}