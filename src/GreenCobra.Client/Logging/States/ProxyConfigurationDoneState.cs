using GreenCobra.Proxy;

namespace GreenCobra.Client.Logging.States;

public record ProxyConnectionOptionConstructedState(ProxyConnectionOptions ConnectionOptions)
    : ILoggerState
{
    public string Format () =>
        $"Client end point   : {ConnectionOptions.LocalEndPoint}\r\n" +
        $"Maximum connections: {ConnectionOptions.ParallelDegree}";
}