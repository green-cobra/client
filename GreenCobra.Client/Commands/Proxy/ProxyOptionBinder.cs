using System.CommandLine.Binding;
using System.Net;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Common;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyOptionBinder : BinderBase<ProxyConnectionOptions>
{
    protected override ProxyConnectionOptions GetBoundValue(BindingContext context)
    {
        var cancellationToken = context.GetService<CancellationToken>();
        var logger = context.GetService<LoggerAdapter>();
        var serverCommunicationService = context.GetService<ServerCommunicationService>();

        var localEndPoint = GetLocalEndPoint(context);
        var configurationUrl = GetConfigurationUrl(context);
        
        var serverConfig = serverCommunicationService
            .GetServerProxyConfigurationAsync(configurationUrl, cancellationToken)
            .GetAwaiter().GetResult();
        
        Guard.AgainstNull(serverConfig.ServerEndPoint);
        
        var proxyConnectionOptions = new ProxyConnectionOptions(
            serverConfig.ServerEndPoint!, localEndPoint, serverConfig.MaxConnections);

        logger.Log(new ProxyConnectionOptionConstructedState(proxyConnectionOptions));

        return proxyConnectionOptions;
    }

    private static EndPoint GetLocalEndPoint(BindingContext context)
    {
        var localHost = context.GetOption(ProxySymbolsStorage.LocalHostOption);
        var localPort = context.GetOption(ProxySymbolsStorage.LocalPortOption);
        return IPEndPoint.Parse($"{localHost}:{localPort}");
    }

    private static Uri GetConfigurationUrl(BindingContext context)
    {
        var serverUrl = context.GetOption(ProxySymbolsStorage.ServerUrlOption);
        var serverDomainRequest = context.GetOption(ProxySymbolsStorage.ServerDomainRequestOption);
        return new Uri(serverUrl, serverDomainRequest);
    }
}