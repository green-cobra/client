using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Common;

namespace GreenCobra.Client.Services.ServerCommunication;

public class ServerCommunicationService
{
    private readonly LoggerAdapter _logger;
    public ServerCommunicationService(LoggerAdapter logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }

    public async Task<ProxyServerConfigurationDto> GetServerProxyConfigurationAsync(
        Uri configurationUrl, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler);
        var response = await httpClient.GetAsync(configurationUrl, cancellationToken);
      
        response.EnsureSuccessStatusCode();

        var serverConfig = await ParseAndValidateResponseAsync(response, cancellationToken);
        serverConfig.ServerEndPoint = await ResolveProxyServerEndPointAsync(serverConfig!, cancellationToken);
        
        _logger.Log(new ProxyServerConfigurationConstructedState(serverConfig));

        return serverConfig;
    }

    private static async Task<ProxyServerConfigurationDto> ParseAndValidateResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var serverConfig = await response.Content
            .ReadFromJsonAsync<ProxyServerConfigurationDto>(cancellationToken: cancellationToken);

        Guard.AgainstNull(serverConfig);
        Guard.AgainstNull(serverConfig!.ServerUrl);
        Guard.AgainstNull(serverConfig.Domain);

        return serverConfig;
    }

    private static async Task<EndPoint?> ResolveProxyServerEndPointAsync(ProxyServerConfigurationDto serverConfig, CancellationToken cancellationToken)
    {
        var ipAddresses = await Dns.GetHostAddressesAsync(
            serverConfig.ServerUrl.DnsSafeHost, AddressFamily.InterNetwork, cancellationToken);

        return new IPEndPoint(ipAddresses.First(), serverConfig.ServerPort);
    }
}