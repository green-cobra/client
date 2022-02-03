using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Handlers;
using GreenCobra.Client.Commands.Proxy.Services;
using GreenCobra.Client.Infrastructure;
using GreenCobra.Client.Infrastructure.ServerCommunication;
using Microsoft.Extensions.DependencyInjection;

namespace GreenCobra.Client.Commands.Proxy.Infrastructure;

/// <summary>
/// Holds DI configuration for Proxy Command
/// </summary>
public static class ProxyServiceCollection
{
    public static IServiceCollection AddProxyServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandBinder<ProxyCommandParams>, ProxyCommand.ProxyParamsBinder>();
        services.AddTransient<IProxyCommandHandler, ProxyCommandHandler>();

        services.AddTransient<IServerCommunicationService, ServerCommunicationService>();
        services.AddTransient<IProxyService, ProxyService>();

        return services;
    }
}