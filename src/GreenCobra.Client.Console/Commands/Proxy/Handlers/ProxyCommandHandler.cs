using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.Client.Console.Configuration;
using GreenCobra.Client.Console.ProxyStream;

namespace GreenCobra.Client.Console.Commands.Proxy.Handlers;

public class ProxyCommandHandler : ICommandHandler
{
    private ProxyParams _proxyParams;
    private BinderBase<ProxyParams> _binder;
    private CancellationToken _cancellationToken;

    public ProxyCommandHandler(ProxyParams proxyParams, CancellationToken cancellationToken)
    {
        _proxyParams = proxyParams;
        //_binder = binder;
        _cancellationToken = cancellationToken;
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        //_binder.TryGetValue(_binder, context.BindingContext)


        System.Console.WriteLine();

        //context.BindingContext.ParseResult.
        //    //.GetValueForOption()
        //    //.GetValueForArgument()

        //context.ParseResult.

        //await ProxyAsync();

        return Task.FromResult(0);
    }

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

            System.Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
            System.Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

            var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
            var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

            await Task.WhenAll(remoteProxyTask, localProxyTask);
        };

    private async Task<ProxyServerConfiguration> GetProxyServerConfigurationAsync(Uri remoteUrl, string remoteDomainRequest)
    {
        HttpClient httpClient = new() { BaseAddress = remoteUrl };

        // todo: handle exception
        var proxyServerConfig = await httpClient.GetFromJsonAsync<ProxyServerConfiguration>(remoteDomainRequest);

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

