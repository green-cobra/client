using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyConfiguration(
    IPEndPoint ServerEndPoint,
    IPEndPoint LocalApplicationEndPoint,
    int MaxConnections);