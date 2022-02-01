using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Infrastructure.ServerCommunication;

public interface IServerCommunicationService
{
    Task<ProxyConfiguration> GetServerProxyConfigurationAsync(ProxyCommandParams commandParams,
        CancellationToken cancellationToken);
}