using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Common;

namespace GreenCobra.Client.Services.ServerCommunication;

public class GreenCobraProxyServer
{
    //private readonly LoggerAdapter _logger;
    // public GreenCobraProxyServer(LoggerAdapter logger)
    // {
    //     Guard.AgainstNull(logger);
    //     _logger = logger;
    // }

    public ProxyConfigurationResponse GetProxyConfiguration(ProxyConfigurationRequest request,
        CancellationToken cancellationToken) =>
        GetProxyConfigurationAsync(request, cancellationToken).GetAwaiter().GetResult();
    
    public async Task<ProxyConfigurationResponse> GetProxyConfigurationAsync(ProxyConfigurationRequest request, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler);

        var content = new StringContent(JsonSerializer.Serialize(request));
        var response = await httpClient.PostAsync(request.ServerUrl, content, cancellationToken);
        var config = await ParseAndValidateResponseAsync(response, cancellationToken);

        var serverAddress = await DnsNameResolver.GetIpAddressAsync(config.ServerUrl.DnsSafeHost, cancellationToken);
        config = config with {ServerEndPoint = new IPEndPoint(serverAddress, config.ServerPort)};
        
        //_logger.Log(new ProxyServerConfigurationConstructedState(config));

        return config;
    }
    
    private static async Task<ProxyConfigurationResponse> ParseAndValidateResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        
        var serverConfig = await response.Content
            .ReadFromJsonAsync<ProxyConfigurationResponse>(cancellationToken: cancellationToken);

        Guard.AgainstNull(serverConfig);
        Guard.AgainstNull(serverConfig!.ServerUrl);
        Guard.AgainstNull(serverConfig.Domain);

        return serverConfig;
    }
}