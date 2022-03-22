#if !DEBUG
using System.CommandLine;
#endif
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using GreenCobra.Client.Commands;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Services.Proxy;
using GreenCobra.Client.Services.ServerCommunication;
using GreenCobra.Common;
using Microsoft.Extensions.Logging;

string[] commandParams;

#if DEBUG
commandParams = new[]
{
    "proxy",
    // "--domain","green-cobra",
    // "--local-port","57679",
    // "--server-url", "http://localhost:3001/api/v1/tunnel" // configured via docker
    
    
    "-d", "chat",
    "-h", "chat.rit",
    "-p", "443",
    "-s", "http://lox.com:3001/api/v1/tunnel"
};
BuildTimeLogger.LogDebug("Debug Input: " + commandParams.Aggregate((s, s1) => $"{s} {s1}"));
#else
commandParams = args;
#endif

var bootstrap = new GreenCobraRootCommand();
var cmdBuilder = new CommandLineBuilder(bootstrap);

cmdBuilder.AddMiddleware(ConfigureServices);
cmdBuilder.UseExceptionHandler(ExceptionHandler);

var parser = cmdBuilder
    //.UseDefaults()
    .UseVersionOption()
    .UseHelp()
    .UseEnvironmentVariableDirective()
    .UseParseDirective()
    .UseSuggestDirective()
    .RegisterWithDotnetSuggest()
    .UseTypoCorrections()
    .UseParseErrorReporting()
    //.UseExceptionHandler()
    .CancelOnProcessTermination()
    .Build();
await parser.InvokeAsync(commandParams);

async Task ConfigureServices(InvocationContext context, Func<InvocationContext, Task> next)
{
    var loggerFactory = LoggerFactory.Create(builder => builder
        .AddSimpleConsole(opt => opt.IncludeScopes = true)
        .SetMinimumLevel(LogLevel.Debug));
    
    context.BindingContext.AddService(_ => loggerFactory);
    context.BindingContext.AddService(_ => new HttpClient(new RetryHttpHandler()));
    
    context.BindingContext.AddService(provider => new GreenCobraProxyServer(
        loggerFactory.CreateLogger<GreenCobraProxyServer>(), provider.GetService<HttpClient>()));
    context.BindingContext.AddService(provider => new ProxyService(loggerFactory.CreateLogger<ProxyService>()));
    
    await next(context);
}

void ExceptionHandler(Exception ex, InvocationContext context)
{
#if DEBUG
    Console.ForegroundColor = ConsoleColor.Red;
    context.Console.Error.WriteLine(ex.Message);
//    context.Console.Error.WriteLine(ex.StackTrace);
    Console.ResetColor();
#else
    Console.ForegroundColor = ConsoleColor.Red;
    context.Console.Error.WriteLine(ex.Message);

    context.Console.WriteLine("Critical error. Stopping execution ...");
#endif
}