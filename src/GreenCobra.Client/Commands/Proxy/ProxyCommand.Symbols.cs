using System.CommandLine;
using GreenCobra.Client.Services.Configuration;
using GreenCobra.Client.Services.Configuration.Models;

namespace GreenCobra.Client.Commands.Proxy;

public partial class ProxyCommand
{
    internal static Option<string> LocalHostOrAddressOption { get; private set; } = null!;
    internal static Option<int> LocalPortOption { get; private set; } = null!;
    internal static Option<Uri> ServerUrlOption { get; private set; } = null!;
    internal static Option<string> DesiredDomainOption { get; private set; } = null!;

    private void InitializeSymbols()
    {
        var configuration = new ConfigurationDefaultService();
        var defaults = configuration.GetProxyDefaults();
        
        InitializeOptions(defaults);
    }

    private void InitializeOptions(ProxyCommandOptions defaults)
    {
        LocalHostOrAddressOption = new Option<string>(
            new[]
            {
                Resources.Option.LocalHostOrAddress_ShortName,
                Resources.Option.LocalHostOrAddress_LongName
            },
            () => defaults.LocalHost,
            Resources.Option.LocalHostOrAddress_Description);

        LocalPortOption = new Option<int>(
            new[]
            {
                Resources.Option.LocalPort_ShortName,
                Resources.Option.LocalPort_LongName
            },
            () => defaults.LocalPort,
            Resources.Option.LocalPort_Description);

        ServerUrlOption = new Option<Uri>(
            new[]
            {
                Resources.Option.ServerUrl_ShortName,
                Resources.Option.ServerUrl_LongName
            },
            () => defaults.ServerUrl,
            Resources.Option.ServerUrl_Description);

        DesiredDomainOption = new Option<string>(
            new[]
            {
                Resources.Option.DesiredDomain_ShortName,
                Resources.Option.DesiredDomain_LongName
            },
            () => defaults.ServerDomainRequest,
            Resources.Option.DesiredDomain_Description);
        
        AddOption(LocalHostOrAddressOption);
        AddOption(LocalPortOption);
        AddOption(ServerUrlOption);
        AddOption(DesiredDomainOption);
    }
}