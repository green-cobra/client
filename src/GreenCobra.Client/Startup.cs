//using System.CommandLine;
//using GreenCobra.Client.Commands;
//using GreenCobra.Client.Commands.Proxy;
//using GreenCobra.Client.Commands.Proxy.Handlers;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Console;

//namespace GreenCobra.Client;
//public static class Startup
//{
//    public static IServiceCollection ConfigureServices()
//    {
//        var services = new ServiceCollection();

//        services.AddLogging(builder =>
//        {
//            builder
//                //.AddJsonConsole()
//                .AddSimpleConsole(options =>
//                {
//                    options.IncludeScopes = true;
//                    options.ColorBehavior = LoggerColorBehavior.Enabled;
//                    options.SingleLine = true;
//                    options.UseUtcTimestamp = true;
//                });

//            builder.SetMinimumLevel(LogLevel.Debug);
//        });
        
//        services.AddTransient<RootCommand, GreenCobraRootCommand>();
//        services.AddTransient<ProxyCommand>();

//        //services.AddTransient<IProxyCommandHandler, ProxyCommandHandler>(provider => provider.);

//        return services;
//    }
//}