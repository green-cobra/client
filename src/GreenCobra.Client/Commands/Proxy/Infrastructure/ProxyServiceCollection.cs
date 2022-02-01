using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Handlers;
using GreenCobra.Client.Commands.Proxy.Services;
using GreenCobra.Client.Infrastructure.ServerCommunication;
using GreenCobra.Client.Logging;
using Microsoft.Extensions.Configuration;
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

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();
        services.AddSingleton(configuration);

        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            // todo: later this should be configured by profile
#if DEBUG
            builder.AddConsoleFormatter<UIConsoleFormatter, ConsoleFormatterOptions>();
            builder.AddConsole(options =>
            {
                options.FormatterName = "ConsoleUi";
            });
#else
            builder.AddSimpleConsole();
#endif
        });

        services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        services.AddTransient<ICommandBinder<ProxyCommandParams>, ProxyCommand.ProxyParamsBinder>();
        services.AddTransient<IProxyCommandHandler, ProxyCommandHandler>();

        services.AddTransient<IServerCommunicationService, ServerCommunicationService>();
        services.AddTransient<IProxyService, ProxyService>();

        return services;
    }
}