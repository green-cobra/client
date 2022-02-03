using System.CommandLine;
using GreenCobra.Client.Commands.Proxy;
using GreenCobra.Client.Configuration;
using GreenCobra.Client.Infrastructure;
using Microsoft.Extensions.Options;

namespace GreenCobra.Client.Commands;

public class GreenCobraRootCommand : RootCommand
{
    public GreenCobraRootCommand()
    {
        var proxyOptions = AppServiceCollection.GetService<IOptions<ProxyOptions>>();
        AddCommand(new ProxyCommand(proxyOptions));
    }
}