using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Proxy;

namespace GreenCobra.Client.Commands.Proxy.Handlers;

public partial class ProxyCommandHandler : ICommandHandler
{
    private readonly ProxyCommandParams _proxyParams;
    private readonly CancellationToken _cancellationToken;

    public ProxyCommandHandler(ProxyCommandParams proxyParams, CancellationToken cancellationToken)
    {
        _proxyParams = proxyParams;
        _cancellationToken = cancellationToken;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        // init step - retrieving configs
        var proxyServerConfig = await GetProxyServerConfigurationAsync();

        Console.WriteLine(proxyServerConfig.ToString());

        var proxyConfig = new ProxyConfiguration(
            await ResolveProxyServerEndPointAsync(proxyServerConfig),
            _proxyParams.LocalServerEndPoint,
            proxyServerConfig.MaxConnections);

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

        var proxyServerConfig = await response.Content.ReadFromJsonAsync<ProxyConnectionConfiguration>(
                cancellationToken: _cancellationToken);

        // todo: update exception
        if (proxyServerConfig is null)
            throw new ArgumentNullException();

        return proxyServerConfig;
    }

    private async Task<IPEndPoint> ResolveProxyServerEndPointAsync(ProxyConnectionConfiguration connectionConfig)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(
                connectionConfig.ServerUrl.DnsSafeHost, AddressFamily.InterNetwork, _cancellationToken))
            .First();

        return new IPEndPoint(ipAddress, connectionConfig.ServerPort);
    }
}

