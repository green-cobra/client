using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyCommandParams(
    IPEndPoint LocalServerEndPoint,
    Uri RemoteServerUrl,
    string RemoteDomainRequest);