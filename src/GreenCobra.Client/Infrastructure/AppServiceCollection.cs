using GreenCobra.Client.Commands.Proxy.Infrastructure;
using GreenCobra.Client.Configuration;
using GreenCobra.Client.Logging.Adapters;
using GreenCobra.Client.Logging.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GreenCobra.Client.Infrastructure;

public static class AppServiceCollection
{
    private static readonly ServiceProvider Provider = ConfigureServices().BuildServiceProvider();

    public static T GetService<T>() where T : notnull => Provider.GetRequiredService<T>();

    private static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        //var platformSpecificPath
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false) // todo: remove as we only will have green-cobra-config file at runtime
            .AddJsonFile("green-cobra-config.json", optional: false, reloadOnChange: false)
            .Build();
        services.Configure<ProxyOptions>(configuration.GetSection(nameof(ProxyOptions)));

        services.AddSingleton(configuration);

        // todo: create custom logger and remove logger configuration
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));

            // todo: later this should be configured by profile
            //#if DEBUG
            builder.AddConsoleFormatter<UIConsoleFormatter, ConsoleFormatterOptions>();
            builder.AddConsole(options =>
            {
                options.FormatterName = "ConsoleUi";
            });
            //#else
            //            builder.AddSimpleConsole();
            //#endif
        });

        // configure common services
        services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        // configure services per command
        services.AddProxyServices();

        return services;
    }
}
