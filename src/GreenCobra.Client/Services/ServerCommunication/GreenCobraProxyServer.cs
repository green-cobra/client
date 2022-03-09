using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Common;

namespace GreenCobra.Client.Services.ServerCommunication;

public class GreenCobraProxyServer
{
    private readonly LoggerAdapter _logger;
    public GreenCobraProxyServer(LoggerAdapter logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }
    
    public async Task<ProxyConfigurationResponse> GetProxyConfigurationAsync(ProxyConfigurationRequest request, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler);

        var content = new StringContent(JsonSerializer.Serialize(request));
        var response = await httpClient.PostAsync(request.ServerUrl, content, cancellationToken);
        var config = await ParseAndValidateResponseAsync(response, cancellationToken);

        var serverEndPoint = await ResolveProxyServerEndPointAsync(config, cancellationToken); 
        config = config with {ServerEndPoint = serverEndPoint};
        
        _logger.Log(new ProxyServerConfigurationConstructedState(config));

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

    private static async Task<EndPoint?> ResolveProxyServerEndPointAsync(ProxyConfigurationResponse serverConfig, CancellationToken cancellationToken)
    {
        // todo: hardcoded to work with localhost
        if (serverConfig.ServerUrl.ToString().Contains("localhost"))
            return new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverConfig.ServerPort); 
        
        var safeHost = serverConfig.ServerUrl.DnsSafeHost;
        var ipAddresses = await Dns.GetHostAddressesAsync(safeHost, AddressFamily.InterNetwork, cancellationToken);

        return new IPEndPoint(ipAddresses.First(), serverConfig.ServerPort);
    }
}