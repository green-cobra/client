using System.CommandLine;
using GreenCobra.Client.Commands.Proxy;

namespace GreenCobra.Client.Commands;

public class GreenCobraRootCommand : RootCommand
{
    public GreenCobraRootCommand()
    {
        AddCommand(new ProxyCommand());
    }
}