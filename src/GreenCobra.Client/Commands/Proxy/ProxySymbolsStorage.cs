using System.CommandLine;
using GreenCobra.Client.Services.Configuration;

namespace GreenCobra.Client.Commands.Proxy;

public static class ProxySymbolsStorage
{
    public static Option<string> LocalHostOption { get; }
    public static Option<int> LocalPortOption { get; }
    public static Option<Uri> ServerUrlOption { get; }
    public static Option<string> ServerDomainRequestOption { get; }

    static ProxySymbolsStorage()
    {
        var configuration = new ConfigurationDefaultService();
        var defaults = configuration.GetProxyDefaults();

        const string stubDescription = "COMING SOON!";
        LocalHostOption = new Option<string>(
            new[] { "--local-host", "-l" },
            () => defaults.LocalHost,
            stubDescription);

        LocalPortOption = new Option<int>(
            new[] { "--local-port", "-p" },
            () => defaults.LocalPort,
            stubDescription);

        ServerUrlOption = new Option<Uri>(
            new[] { "--server-url", "-s" },
            () => defaults.ServerUrl,
            stubDescription);

        ServerDomainRequestOption = new Option<string>(
            new[] { "--server-domain-request", "-d" },
            () => defaults.ServerDomainRequest,
            stubDescription);
    }

    public static void RegisterOptions(this ProxyCommand command)
    {
        command.AddOption(LocalHostOption);
        command.AddOption(LocalPortOption);
        command.AddOption(ServerUrlOption);
        command.AddOption(ServerDomainRequestOption);
    }
}