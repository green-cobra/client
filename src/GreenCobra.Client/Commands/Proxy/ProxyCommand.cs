using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;
using GreenCobra.Client.Commands.Proxy.Models;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Client.Services.ServerCommunication.Models;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Commands.Proxy;

public partial class ProxyCommand : Command
{
    public ProxyCommand() :base(Resources.ProxyCommand_Name, Resources.ProxyCommand_Description)
    {
        InitializeSymbols();
        
        this.SetHandler<ProxyCommandInput, InvocationContext, CancellationToken>(ProxyHandler, new ProxyCommandBinder());
    }

    private async Task ProxyHandler(ProxyCommandInput input, InvocationContext invocationContext, CancellationToken cancellationToken)
    {
        var context = invocationContext.BindingContext;
        var proxyServer = context.GetService<GreenCobraProxyServer>();
        var proxyService = context.GetService<ProxyService>();
        var logger = context.GetService<ILoggerFactory>().CreateLogger<ProxyCommand>();
        
        logger.LogDebug(Resources.Traces.ProxyHandler_Enter);
        
        var proxyPoint =
            await proxyServer.SetupProxyPointAsync(new ProxyPointSetupRequest(input.ServerUrl, input.DesiredDomain),
                cancellationToken);
        
        logger.LogInformation(Resources.Logs.ProxyPointSetupDone, proxyPoint.DistributedDomain, proxyPoint.ParallelConnections, proxyPoint.ProxyPoinHost, proxyPoint.ProxyPointPort);

        var localEndPointTask = DnsNameResolver.GetIpAddressAsync(input.LocalHostOrAddress, logger, cancellationToken);
        var proxyPointEndPointTask = DnsNameResolver.GetIpAddressAsync(proxyPoint.ProxyPoinHost.DnsSafeHost, logger, cancellationToken);
        await TaskExt.WhenAll(localEndPointTask, proxyPointEndPointTask);
        var localAddress = await localEndPointTask;
        var proxyPointAddress = await proxyPointEndPointTask;

        var localEndPoint = new IPEndPoint(localAddress, input.LocalPort);
        var proxyPointEndPoint = new IPEndPoint(proxyPointAddress, proxyPoint.ProxyPointPort);
        
        logger.LogInformation(Resources.Logs.ProxyEndpointsResolved, proxyPointEndPoint, localEndPoint);
        
        await proxyService.StartProxyAsync(localEndPoint, proxyPointEndPoint, proxyPoint.ParallelConnections, cancellationToken);
    }
}