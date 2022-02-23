using GreenCobra.Common;

namespace GreenCobra.Client.Helpers;

public static class ServiceProviderExtensions
{
    public static T GetService<T>(this IServiceProvider sp)
    {
        var service = sp.GetService(typeof(T));

        Guard.AgainstNull(service);

        return (T) service!;
    }
}