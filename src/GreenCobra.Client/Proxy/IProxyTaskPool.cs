using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Proxy;

public interface IProxyTaskPool
{
    Task RunAsync(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken);
}