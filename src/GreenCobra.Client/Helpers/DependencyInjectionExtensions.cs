using System.CommandLine;
using System.CommandLine.Binding;
using GreenCobra.Common;

namespace GreenCobra.Client.Helpers;

public static class DependencyInjectionExtensions
{
    public static T GetService<T>(this BindingContext ctx)
    {
        var service = ctx.GetService(typeof(T));
        Guard.AgainstNull(service);

        return (T) service;
    }

    public static T GetService<T>(this IServiceProvider sp)
    {
        var service = sp.GetService(typeof(T));
        Guard.AgainstNull(service);

        return (T) service;
    }
    
    public static T GetOption<T>(this BindingContext context, Option<T> option) =>
        context.ParseResult.GetValueForOption(option)!;
}