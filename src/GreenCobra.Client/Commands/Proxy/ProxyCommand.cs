using System.CommandLine;
using System.CommandLine.Invocation;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommand : Command
{
    private new const string Name = "proxy";
    private new const string Description = "Proxy requests from public server to your locally running application";

    public ProxyCommand() :base(Name, Description)
    {
        this.RegisterOptions();
        this.SetHandler<ProxyConfiguration, InvocationContext, CancellationToken>(ProxyHandler, new ProxyCommandBinder());
    }

    private async Task ProxyHandler(ProxyConfiguration proxyConfig, InvocationContext ctx, CancellationToken cancellationToken)
    {
        // wait until app will not be closed
        var proxyService = ctx.BindingContext.GetService<ProxyService>();
        await proxyService.StartProxyAsync(proxyConfig, cancellationToken);
    }
}