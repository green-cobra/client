using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyConfiguration(
    EndPoint ServerEndPoint,
    EndPoint ClientEndPoint,
    int MaxConnections);