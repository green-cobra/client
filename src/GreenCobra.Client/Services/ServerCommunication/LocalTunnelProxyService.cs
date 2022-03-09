using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Common;

namespace GreenCobra.Client.Services.ServerCommunication;

public class LocalTunnelProxyService
{
    private readonly LoggerAdapter _logger;
    public LocalTunnelProxyService(LoggerAdapter logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }

    public async Task<ProxyConfigurationResponse> GetProxyConfigurationAsync(
        Uri configurationUrl, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler);
        var response = await httpClient.GetAsync(configurationUrl, cancellationToken);
        var config = await ParseAndValidateResponseAsync(response, cancellationToken);

        config = config with {ServerEndPoint = await ResolveProxyServerEndPointAsync(config, cancellationToken)};
        
        _logger.Log(new ProxyServerConfigurationConstructedState(config));

        return config;
    }

    private static async Task<ProxyConfigurationResponse> ParseAndValidateResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        
        var serverConfig = await response.Content
            .ReadFromJsonAsync<ProxyConfigurationResponse>(cancellationToken: cancellationToken);

        Guard.AgainstNull(serverConfig);
        Guard.AgainstNull(serverConfig!.ServerHost);
        Guard.AgainstNull(serverConfig.Domain);
        Guard.Satisfy(serverConfig.MaxConnections > 0);
        
        return serverConfig;
    }

    private static async Task<EndPoint> ResolveProxyServerEndPointAsync(ProxyConfigurationResponse serverConfig, CancellationToken cancellationToken)
    {
        var safeHost = serverConfig.ServerHost.DnsSafeHost; 
        var ipAddresses = await Dns.GetHostAddressesAsync(safeHost, AddressFamily.InterNetwork, cancellationToken);

        return new IPEndPoint(ipAddresses.First(), serverConfig.ServerPort);
    }
}