using System.CommandLine;
using GreenCobra.Client.Console.Commands.Proxy;

namespace GreenCobra.Client.Console.Commands;

public class GreenCobraRootCommand : RootCommand
{
    public GreenCobraRootCommand()
    {
        AddCommand(new ProxyCommand());
    }
}