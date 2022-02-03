using System.CommandLine;
using GreenCobra.Client.Commands;

string[] commandParams;

#if DEBUG
commandParams = new[]
{
    $"green-cobra proxy " +
    $"--remote-domain-request green-cobra-7576 " +
    $"--local-port 57679 "
};
#else
commandParams = args;
#endif

var bootstrap = new GreenCobraRootCommand();
await bootstrap.InvokeAsync(commandParams);