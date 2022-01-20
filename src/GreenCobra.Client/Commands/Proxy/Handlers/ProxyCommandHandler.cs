using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Configuration;
using GreenCobra.Client.ProxyStream;

namespace GreenCobra.Client.Commands.Proxy.Handlers;

public class ProxyCommandHandler : ICommandHandler
{
    private readonly ProxyParams _proxyParams;
    private CancellationToken _cancellationToken;

    private readonly ProxyTaskPool _taskPool;

    public ProxyCommandHandler(ProxyParams proxyParams, CancellationToken cancellationToken)
    {
        _proxyParams = proxyParams;
        _cancellationToken = cancellationToken;
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {

        _taskPool = new ProxyTaskPool(
            serverConfiguration.ConnectionLimit,
            ProxyFunc(serverConfiguration, localEndPoint));


        //context.BindingContext.ParseResult.
        //    //.GetValueForOption()
        //    //.GetValueForArgument()

        //context.ParseResult.

        //await ProxyAsync();

        return Task.FromResult(0);

        await _taskPool.RunAsync(CancellationToken.None);
    }

    private static Func<Task> ProxyFunc(ProxyServerConfiguration serverConfiguration, IPEndPoint localEndPoint) =>
        async () =>
        {
            using var remoteProxyStream = new ProxyStream.ProxyStream(localEndPoint, ProxyStreamType.Remote);
            using var localProxyStream = new ProxyStream.ProxyStream(serverConfiguration.IpEndPoint, ProxyStreamType.Local);

            Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
            Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

            var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
            var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

            await Task.WhenAll(remoteProxyTask, localProxyTask);
        };

    private async Task ProxyAsync(ProxyParams proxyParams)
    {
        // init step - retrieving configs
        //var localEndPoint = IPEndPoint.Parse($"{proxyParams.}:{proxyParams.LocalServerPort}");
        var proxyServerConfig =
            await GetProxyServerConfigurationAsync(proxyParams.RemoteServerUrl, proxyParams.RemoteDomainRequest);

        // log to console
        //Console.WriteLine(localEndPoint.ToString());
        //Console.WriteLine(proxyServerConfig.ToString());

        // init Task pool
        var taskPool = new ProxyTaskPool(proxyServerConfig.ConnectionLimit,
            ProxyFunc(proxyServerConfig, _proxyParams.LocalServerEndPoint));

        // wait until app will not be closed
        await taskPool.RunAsync(CancellationToken.None);
    }

    private static Func<Task> ProxyFunc(ProxyServerConfiguration serverConfiguration, IPEndPoint localEndPoint) =>
        async () =>
        {
            using var remoteProxyStream = new ProxyStream.ProxyStream(localEndPoint, ProxyStreamType.Remote);
            using var localProxyStream = new ProxyStream.ProxyStream(serverConfiguration.IpEndPoint, ProxyStreamType.Local);

            Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
            Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

            var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
            var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

            await Task.WhenAll(remoteProxyTask, localProxyTask);
        };

    public class RetryHttpHandler : DelegatingHandler
    {
        // todo: put to config
        private readonly int _maxRetries = 3;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            for (int i = 0; i < _maxRetries; i++)
            {
                if (response.IsSuccessStatusCode)
                    return response;

                // todo: add logic for some status codes  response.StatusCode == 
            }

            return response;
        }
    }

    private async Task<ProxyServerConfiguration> GetProxyServerConfigurationAsync()
    {
        using var httpClient = new HttpClient(new RetryHttpHandler()) {BaseAddress = _proxyParams.RemoteServerUrl};

        var response = await httpClient.GetAsync(_proxyParams.RemoteDomainRequest);

        response.EnsureSuccessStatusCode();
        
        response.

        proxyServerConfig = await httpClient.GetFromJsonAsync<ProxyServerConfiguration>(_proxyParams.RemoteDomainRequest);

        proxyServerConfig.IpEndPoint = await ResolveRemoteAddressAsync(proxyServerConfig);

        return proxyServerConfig;
    }

    private async Task<IPEndPoint> ResolveRemoteAddressAsync(ProxyServerConfiguration proxyServerConfig)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(
                proxyServerConfig.Url.DnsSafeHost,
                AddressFamily.InterNetwork))
            .First();

        return new IPEndPoint(ipAddress, proxyServerConfig.Port);
    }
}

