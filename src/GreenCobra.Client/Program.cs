using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GreenCobra.Client.Commands;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Client.Services.ServerCommunication;

string[] commandParams;

#if DEBUG
commandParams = new[]
{
    "proxy",
    "--server-domain-request","green-cobra-7576",
    "--local-port","57679"
};
Console.WriteLine("Debug Input: " + commandParams.Aggregate((s, s1) => $"{s} {s1}"));
#else
commandParams = args;
#endif

var bootstrap = new GreenCobraRootCommand();

var cmdBuilder = new CommandLineBuilder(bootstrap);
cmdBuilder.AddMiddleware(async (context, next) =>
{
    context.BindingContext.AddService(provider => new LoggerAdapter(provider.GetService<IConsole>()));
    context.BindingContext.AddService(provider => new ServerCommunicationService(provider.GetService<LoggerAdapter>()));
    context.BindingContext.AddService(provider => new ProxyService(provider.GetService<LoggerAdapter>()));

    await next(context);
});

var parser = cmdBuilder.UseDefaults().Build();
await parser.InvokeAsync(commandParams);