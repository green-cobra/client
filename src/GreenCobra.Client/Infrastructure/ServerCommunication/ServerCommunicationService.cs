using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Infrastructure.ServerCommunication.Models;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;

namespace GreenCobra.Client.Infrastructure.ServerCommunication;

public class ServerCommunicationService : IServerCommunicationService
{
    private readonly ILoggerAdapter<ServerCommunicationService> _logger;
    public ServerCommunicationService(ILoggerAdapter<ServerCommunicationService> logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }

    public async Task<ProxyConfiguration> GetServerProxyConfigurationAsync(
        ProxyCommandParams commandParams, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler) { BaseAddress = commandParams.ServerUrl };
        var response = await httpClient.GetAsync(commandParams.RemoteDomainRequest, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var proxyServerConfig = await response.Content
            .ReadFromJsonAsync<ProxyServerConfigurationDto>(cancellationToken: cancellationToken);

        ValidateProxyServerConfiguration(proxyServerConfig);
        
        var serverEndPoint = await ResolveProxyServerEndPointAsync(proxyServerConfig!, cancellationToken);
        var proxyConfig = new ProxyConfiguration(
            serverEndPoint,
            commandParams.ApplicationEndPoint,
            proxyServerConfig!.MaxConnections);

        _logger.LogInformation(new GotProxyConfigurationState
        {
            ProxyConfiguration = proxyConfig,
            ServerUrl = commandParams.ServerUrl
        });

        return proxyConfig;
    }

    private static void ValidateProxyServerConfiguration(ProxyServerConfigurationDto? proxyServerConfig)
    {
        Guard.AgainstNull(proxyServerConfig);
        Guard.AgainstNull(proxyServerConfig.ServerUrl);
        Guard.AgainstNull(proxyServerConfig.Domain);
    }

    private static async Task<EndPoint> ResolveProxyServerEndPointAsync(ProxyServerConfigurationDto serverConfig, CancellationToken cancellationToken)
    {
        var ipAddresses = await Dns.GetHostAddressesAsync(
            serverConfig.ServerUrl.DnsSafeHost, AddressFamily.InterNetwork, cancellationToken);

        return new IPEndPoint(ipAddresses.First(), serverConfig.ServerPort);
    }
}