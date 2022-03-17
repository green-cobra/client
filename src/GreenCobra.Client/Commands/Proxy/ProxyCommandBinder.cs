using System.CommandLine.Binding;
using GreenCobra.Client.Commands.Proxy.Models;
using GreenCobra.Client.Helpers;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommandBinder : BinderBase<ProxyCommandInput>
{
    protected override ProxyCommandInput GetBoundValue(BindingContext context)
    {
        var logger = context.GetService<ILoggerFactory>().CreateLogger<ProxyCommandBinder>();

        var localHostOrAddress = context.GetOption(ProxyCommand.LocalHostOrAddressOption);
        var localPort = context.GetOption(ProxyCommand.LocalPortOption);
        var serverUrl = context.GetOption(ProxyCommand.ServerUrlOption);
        var desiredDomain = context.GetOption(ProxyCommand.DesiredDomainOption);

        logger.LogInformation(Resources.Logs.ProxyCommandInputParsed, localHostOrAddress, localPort, serverUrl, desiredDomain);
        
        return new ProxyCommandInput(localHostOrAddress, localPort, serverUrl, desiredDomain);
    }
}