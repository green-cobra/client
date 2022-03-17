namespace GreenCobra.Client.Commands.Proxy.Models;

public record ProxyCommandInput(
    string LocalHostOrAddress,
    int LocalPort,
    Uri ServerUrl,
    string DesiredDomain)
{
    // todo: fix defaults
    public static ProxyCommandInput Defaults => 
        new("localhost", 80, new Uri("https://localtunnel.me/"), "?new");
};