using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GreenCobra.Client.Commands;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Client.Services.ServerCommunication;
using Microsoft.Extensions.Logging;

string[] commandParams;

#if DEBUG
commandParams = new[]
{
    "proxy",
    //"--local-host", "chat.rit",
    "--domain","green-cobra-7578",
    "--local-port","57679",
    "--server-url", "http://localhost:3001/api/v1/tunnel" // configured via docker
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
    context.BindingContext.AddService(_ => new GreenCobraProxyServer());
    context.BindingContext.AddService(provider => new ProxyService(provider.GetService<LoggerAdapter>()));
    
    context.BindingContext.AddService(_ => LoggerFactory.Create(builder => builder.AddSimpleConsole()));

    await next(context);
});

var parser = cmdBuilder.UseDefaults().Build();
await parser.InvokeAsync(commandParams);