using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Handlers;
using GreenCobra.Client.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GreenCobra.Client.Commands.Proxy.Infrastructure;

/// <summary>
/// Holds DI configuration for Proxy Command
/// </summary>
public static class ProxyServiceCollection
{
    public static ServiceProvider BuildServices()
    {
        var services = ConfigureServices();
        return services.BuildServiceProvider();
    }

    private static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder
                //.AddJsonConsole()
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                    options.SingleLine = true;
                    options.UseUtcTimestamp = true;
                });

            builder.SetMinimumLevel(LogLevel.Debug);
        });


        services.AddTransient<IProxyTaskPool, ProxyTaskPool>();

        services.AddTransient<ICommandBinder<ProxyCommandParams>, ProxyCommand.ProxyParamsBinder>();
        services.AddTransient<IProxyCommandHandler, ProxyCommandHandler>();

        return services;
    }
}