using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.ConsoleUI;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Proxy;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Commands.Proxy.Handlers;

public class ProxyCommandHandler : ICommandHandler
{
    private readonly ProxyCommandParams _proxyParams;
    private readonly CancellationToken _cancellationToken;

    private readonly ILogger<ProxyCommandHandler> _logger;
    
    public ProxyCommandHandler(ProxyCommandParams proxyParams, CancellationToken cancellationToken)
    {
        _proxyParams = proxyParams;
        _cancellationToken = cancellationToken;

        _logger = CommandLoggerFactory.GetLogger<ProxyCommandHandler>();
        _logger.LogDebug($"{_proxyParams}");
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        // init step - retrieving configs
        var proxyConnectionConfig = await GetProxyServerConfigurationAsync();

        var proxyServerEndPoint = await ResolveProxyServerEndPointAsync(proxyConnectionConfig);
        var proxyConfig = new ProxyConfiguration(
            proxyServerEndPoint,
            _proxyParams.LocalServerEndPoint,
            proxyConnectionConfig.MaxConnections);

        Dashboard.DisplayProxyConnectionConfiguration(proxyConnectionConfig);
        _logger.LogDebug($"Proxy server {proxyServerEndPoint}");

        // init Task pool
        var taskPool = new ProxyTaskPool(proxyConfig);

        // wait until app will not be closed
        await taskPool.RunAsync(_cancellationToken);

        // todo: resolve status code
        return 0;
    }

    private async Task<ProxyConnectionConfiguration> GetProxyServerConfigurationAsync()
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler) {BaseAddress = _proxyParams.RemoteServerUrl};
        var response = await httpClient.GetAsync(_proxyParams.RemoteDomainRequest, _cancellationToken);

        response.EnsureSuccessStatusCode();

        var proxyConnectionConfig = await response.Content.ReadFromJsonAsync<ProxyConnectionConfiguration>(
                cancellationToken: _cancellationToken);

        // todo: update exception
        if (proxyConnectionConfig is null)
            throw new ArgumentNullException();

        _logger.LogDebug($"{proxyConnectionConfig}");

        return proxyConnectionConfig;
    }

    private async Task<IPEndPoint> ResolveProxyServerEndPointAsync(ProxyConnectionConfiguration connectionConfig)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(
                connectionConfig.ServerUrl.DnsSafeHost, AddressFamily.InterNetwork, _cancellationToken))
            .First();
        
        return new IPEndPoint(ipAddress, connectionConfig.ServerPort);
    }
}

