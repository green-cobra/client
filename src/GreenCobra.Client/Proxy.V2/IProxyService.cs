using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Proxy.V2;

public interface IProxyService
{
    Task StartProxyAsync(ProxyConfiguration configuration, CancellationToken cancellationToken);
}