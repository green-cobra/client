using System.Net;

namespace GreenCobra.Client.Commands.Proxy;

public record ProxyParams(IPEndPoint LocalServerEndPoint, Uri RemoteServerUrl, string RemoteDomainRequest);