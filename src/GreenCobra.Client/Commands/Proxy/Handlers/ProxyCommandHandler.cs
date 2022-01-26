using System.CommandLine.Binding;
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

public interface IProxyCommandHandler : ICommandHandler
{

}

public class ProxyCommandHandler : IProxyCommandHandler // ICommandHandler
{
    private ProxyCommandParams _proxyParams;
    //private CancellationToken _cancellationToken;

    private readonly ICommandBinder<ProxyCommandParams> _paramsBinder;
    private readonly ILogger<ProxyCommandHandler> _logger;
    private readonly IProxyTaskPool _proxyTaskPool;

    public ProxyCommandHandler(
        //ProxyCommandParams proxyParams,
        //CancellationToken cancellationToken,
        ICommandBinder<ProxyCommandParams> paramsBinder,
        IProxyTaskPool proxyTaskPool,
        ILogger<ProxyCommandHandler> logger)
    {
        //_proxyParams = proxyParams;
        //_cancellationToken = cancellationToken;
        _proxyTaskPool = proxyTaskPool;

        _logger = logger;
        _paramsBinder = paramsBinder;
        //_logger.LogDebug($"{_proxyParams}");
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        //var symbol = new ProxyCommand.ProxyParamsBinder();
        //var @params = symbol.BindParametersFromContext(context.BindingContext);
        _proxyParams = _paramsBinder.BindParametersFromContext(context.BindingContext);
        var cancellationToken = context.GetCancellationToken();

        // init step - retrieving configs
        var proxyConnectionConfig = await GetProxyServerConfigurationAsync(cancellationToken);

        var proxyServerEndPoint = await ResolveProxyServerEndPointAsync(proxyConnectionConfig, cancellationToken);
        var proxyConfig = new ProxyConfiguration(
            proxyServerEndPoint,
            _proxyParams.LocalServerEndPoint,
            proxyConnectionConfig.MaxConnections);

        _logger.LogDebug($"Proxy server {proxyServerEndPoint}");

        // init Task pool
        //var taskPool = new ProxyTaskPool(proxyConfig);

        // wait until app will not be closed
        await _proxyTaskPool.RunAsync(proxyConfig, cancellationToken);

        // todo: resolve status code
        return context.ExitCode;
    }

    private async Task<ProxyConnectionConfiguration> GetProxyServerConfigurationAsync(CancellationToken cancellationToken)
    {
        using var retryHandler = new RetryHttpHandler();
        using var httpClient = new HttpClient(retryHandler) {BaseAddress = _proxyParams.RemoteServerUrl};
        var response = await httpClient.GetAsync(_proxyParams.RemoteDomainRequest, cancellationToken);

        response.EnsureSuccessStatusCode();

        var proxyConnectionConfig = await response.Content.ReadFromJsonAsync<ProxyConnectionConfiguration>(
                cancellationToken: cancellationToken);

        // todo: update exception
        if (proxyConnectionConfig is null)
            throw new ArgumentNullException();

        _logger.LogDebug($"{proxyConnectionConfig}");

        return proxyConnectionConfig;
    }

    private async Task<IPEndPoint> ResolveProxyServerEndPointAsync(
        ProxyConnectionConfiguration connectionConfig, CancellationToken cancellationToken)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(
                connectionConfig.ServerUrl.DnsSafeHost, AddressFamily.InterNetwork, cancellationToken))
            .First();
        
        return new IPEndPoint(ipAddress, connectionConfig.ServerPort);
    }
}

