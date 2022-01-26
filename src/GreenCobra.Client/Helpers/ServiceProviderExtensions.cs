using Microsoft.Extensions.DependencyInjection;

namespace GreenCobra.Client.Helpers;

public static class ServiceProviderExtensions
{
    public static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters)
    {
        // performance hit because on object[] and boxing.
        // Could be resolved using .CreateFactory but seems not urgent as no load expected in this app
        return ActivatorUtilities.CreateInstance<T>(provider, parameters);
    }
}