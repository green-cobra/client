using System.Net;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyCommandParams(
    IPEndPoint LocalApplicationEndPoint,
    Uri RemoteServerUrl,
    string RemoteDomainRequest);