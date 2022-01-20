using System.Net;

namespace GreenCobra.Client.Console.Commands.Proxy;

public record ProxyParams(IPEndPoint LocalServerEndPoint, Uri RemoteServerUrl, string RemoteDomainRequest);