using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging.States.Interfaces;

namespace GreenCobra.Client.Logging.States;

public record GotProxyConfigurationState(ProxyConfiguration ProxyConfiguration, Uri ServerUrl)
    : IState, IStateFormatter<GotProxyConfigurationState>
{
    public LoggingEventId EventId => LoggingEventId.GotServerConfiguration;

    public Func<GotProxyConfigurationState, Exception?, string> Formatter { get; } = (state, exception) => 
        $"Proxy server configuration:\r\n" +
        $"Server URL         : {state.ServerUrl}\r\n" +
        $"Server end point   : {state.ProxyConfiguration.ServerEndPoint}\r\n" +
        $"Client end point   : {state.ProxyConfiguration.ClientEndPoint}\r\n" +
        $"Maximum connections: {state.ProxyConfiguration.MaxConnections}\r\n" +
        $"\r\n";
}