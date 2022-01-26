using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Proxy;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Commands.Proxy.Handlers;

public class ProxyCommandHandler : IProxyCommandHandler
{
    private readonly ICommandBinder<ProxyCommandParams> _paramsBinder;
    private readonly ILogger<ProxyCommandHandler> _logger;
    private readonly IProxyTaskPool _proxyTaskPool;

    public ProxyCommandHandler(
        ICommandBinder<ProxyCommandParams> paramsBinder,
        IProxyTaskPool proxyTaskPool,
        ILogger<ProxyCommandHandler> logger)
    {
        _paramsBinder = paramsBinder;
        _proxyTaskPool = proxyTaskPool;
        _logger = logger;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var commandParams = _paramsBinder.BindParametersFromContext(context.BindingContext);
        var cancellationToken = context.GetCancellationToken();

        // init step - retrieving configs
        var connectionConfig = await GetConnectionConfigurationAsync(commandParams, cancellationToken);
        
        _logger.LogDebug($"Connection info: {connectionConfig}");

        var proxyServerEndPoint = await ResolveProxyServerEndPointAsync(connectionConfig, cancellationToken);
        var proxyConfig = new ProxyConfiguration(
            proxyServerEndPoint,
            commandParams.LocalApplicationEndPoint,
            connectionConfig.MaxConnections);

        // wait until app will not be closed
        await _proxyTaskPool.RunAsync(proxyConfig, cancellationToken);

        return context.ExitCode;
    }

    // todo: this can be moved to separate service when we will resolve LocalTunnel server and our
    private async Task<ProxyConnectionConfiguration> GetConnectionConfigurationAsync(ProxyCommandParams commandParams, CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler) {BaseAddress = commandParams.RemoteServerUrl};
        var response = await httpClient.GetAsync(commandParams.RemoteDomainRequest, cancellationToken);

        response.EnsureSuccessStatusCode();

        var proxyConnectionConfig = await response.Content.ReadFromJsonAsync<ProxyConnectionConfiguration>(
                cancellationToken: cancellationToken);

        // todo: update exception
        if (proxyConnectionConfig is null)
            throw new ArgumentNullException();

        return proxyConnectionConfig;
    }

    private async Task<IPEndPoint> ResolveProxyServerEndPointAsync(ProxyConnectionConfiguration connectionConfig, CancellationToken cancellationToken)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(connectionConfig.ServerUrl.DnsSafeHost,
            AddressFamily.InterNetwork, cancellationToken)).First();
        
        return new IPEndPoint(ipAddress, connectionConfig.ServerPort);
    }
}

