using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Commands.Proxy.Services;

public interface IProxyService
{
    Task StartProxyAsync(ProxyConfiguration configuration, CancellationToken cancellationToken);
}