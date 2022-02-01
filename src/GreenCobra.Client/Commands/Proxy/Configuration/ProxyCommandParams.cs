using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyCommandParams(
    IPEndPoint ApplicationEndPoint,
    Uri ServerUrl,
    string RemoteDomainRequest);