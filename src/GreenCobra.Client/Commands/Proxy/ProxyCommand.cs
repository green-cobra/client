using System.CommandLine;
using System.CommandLine.Invocation;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Commands.Proxy;

public partial class ProxyCommand : Command
{
    public ProxyCommand() :base(Resources.ProxyCommand_Name, Resources.ProxyCommand_Description)
    {
        InitializeSymbols();
        
        this.SetHandler<ProxyConfiguration, InvocationContext, CancellationToken>(ProxyHandler, new ProxyCommandBinder());
    }

    private async Task ProxyHandler(ProxyConfiguration proxyConfig, InvocationContext ctx, CancellationToken cancellationToken)
    {
        var proxyService = ctx.BindingContext.GetService<ProxyService>();
        
        await proxyService.StartProxyAsync(proxyConfig, cancellationToken);
    }
}