using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyConfiguration(
    IPEndPoint ServerEndPoint,
    IPEndPoint ClientEndPoint,
    int MaxConnections);