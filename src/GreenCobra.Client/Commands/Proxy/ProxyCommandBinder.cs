using System.CommandLine.Binding;
using System.Net;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Proxy;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommandBinder : BinderBase<ProxyConfiguration>
{
    protected override ProxyConfiguration GetBoundValue(BindingContext context)
    {
        // retrieve injected services
        var cancellationToken = context.GetService<CancellationToken>();
        var proxyServer = context.GetService<GreenCobraProxyServer>();
        var logger = context.GetService<ILoggerFactory>().CreateLogger<ProxyConfiguration>();

        var (localHostOrAddress, localPort, serverUrl, desiredDomain) = ParseCommandInput(context);
        
        var proxyRequest = new ProxyConfigurationRequest(serverUrl, desiredDomain);
        var (serverEndPoint, maxConnections) = proxyServer.GetProxyConfiguration(proxyRequest, cancellationToken);
        
        var localAddress = DnsNameResolver.GetIpAddress(localHostOrAddress, cancellationToken);
        var localEndPoint = IPEndPoint.Parse($"{localAddress}:{localPort}");
        var proxyConfig = new ProxyConfiguration(serverEndPoint!, localEndPoint, maxConnections);

        logger.LogInformation(Resources.Messages.ProxyCommandInputParsed, localHostOrAddress, localPort, serverUrl, desiredDomain);
        logger.LogInformation(Resources.Messages.ProxyOptionsRetrieved, proxyConfig.LocalEndPoint, proxyConfig.ParallelDegree);
        
        return proxyConfig;
    }

    private (string LocalHostOrAddress, int LocalPort, Uri ServerUrl, string DesiredDomain) ParseCommandInput(BindingContext context)
    {
        var localHostOrAddress = context.GetOption(ProxyCommand.LocalHostOrAddressOption);
        var localPort = context.GetOption(ProxyCommand.LocalPortOption);
        var serverUrl = context.GetOption(ProxyCommand.ServerUrlOption);
        var desiredDomain = context.GetOption(ProxyCommand.DesiredDomainOption);

        return (localHostOrAddress, localPort, serverUrl, desiredDomain);
    }
}