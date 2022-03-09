using System.CommandLine.Binding;
using System.Net;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommandBinder : BinderBase<ProxyConfiguration>
{
    protected override ProxyConfiguration GetBoundValue(BindingContext context)
    {
        var cancellationToken = context.GetService<CancellationToken>();
        var logger = context.GetService<LoggerAdapter>();
        var ltServer = context.GetService<LocalTunnelProxyService>();

        var localEndPoint = BindLocalEndPoint(context);
        
        var configurationUrl = BindConfigurationUrl(context);
        var (serverEndPoint, maxConnections) = ltServer
            .GetProxyConfigurationAsync(configurationUrl, cancellationToken)
            .GetAwaiter().GetResult();
        
        var proxyConfig = new ProxyConfiguration(serverEndPoint!, localEndPoint, maxConnections);

        logger.Log(new ProxyConnectionOptionConstructedState(proxyConfig));

        return proxyConfig;
    }

    private static EndPoint BindLocalEndPoint(BindingContext context)
    {
        var localHost = context.GetOption(ProxySymbolsStorage.LocalHostOption);
        var localPort = context.GetOption(ProxySymbolsStorage.LocalPortOption);
        return IPEndPoint.Parse($"{localHost}:{localPort}");
    }

    private static Uri BindConfigurationUrl(BindingContext context)
    {
        var serverUrl = context.GetOption(ProxySymbolsStorage.ServerUrlOption);
        var serverDomainRequest = context.GetOption(ProxySymbolsStorage.ServerDomainRequestOption);
        return new Uri(serverUrl, serverDomainRequest);
    }
}