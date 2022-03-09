using System.CommandLine.Binding;
using System.Net;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommandBinder : BinderBase<ProxyConfiguration>
{
    protected override ProxyConfiguration GetBoundValue(BindingContext context)
    {
        var cancellationToken = context.GetService<CancellationToken>();
        var logger = context.GetService<LoggerAdapter>();
        var proxyServer = context.GetService<GreenCobraProxyServer>();

        var localEndPoint = BindLocalEndPoint(context);
        
        var serverUrlUrl = BindServerUrl(context);
        var domainRequest = BindDomainRequest(context);
        var proxyRequest = new ProxyConfigurationRequest(serverUrlUrl, domainRequest); 
        
        var (serverEndPoint, maxConnections) = proxyServer
            .GetProxyConfigurationAsync(proxyRequest, cancellationToken)
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

    private static Uri BindServerUrl(BindingContext context) => context.GetOption(ProxySymbolsStorage.ServerUrlOption);
    private static string BindDomainRequest(BindingContext context) => context.GetOption(ProxySymbolsStorage.ServerDomainRequestOption);
}