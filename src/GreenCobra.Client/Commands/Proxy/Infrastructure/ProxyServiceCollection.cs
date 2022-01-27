﻿using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Handlers;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            builder.AddSimpleConsole();
        });


        services.AddTransient<ILoggerFormatter<string>, StringLoggerFormatter>();
        services.AddTransient<ILoggerFormatter<byte[]>, BinaryLoggerFormatter>();
        
        services.AddTransient(typeof(ILoggerAdapter<,>),
            typeof(LoggerAdapter<,>));

        services.AddTransient<IProxyTaskPool, ProxyTaskPool>();
        services.AddTransient<ICommandBinder<ProxyCommandParams>, ProxyCommand.ProxyParamsBinder>();
        services.AddTransient<IProxyCommandHandler, ProxyCommandHandler>();

        //services.AddTransient(provider => provider.ResoleWith<ProxyStream>());


        return services;
    }

    private static T ResoleWith<T>(this IServiceProvider provider, params object[] args)
    {
        return ActivatorUtilities.CreateInstance<T>(provider, typeof(T), args);
    }
}