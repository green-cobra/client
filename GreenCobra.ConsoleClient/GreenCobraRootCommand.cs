using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using GreenCobra.ConsoleClient.Configuration;
using GreenCobra.ConsoleClient.ProxyStream;

namespace GreenCobra.ConsoleClient;

public class GreenCobraRootCommand : RootCommand
{
    public GreenCobraRootCommand()
    {
        const string description = "COMING SOON!";

        Name = "green-cobra";
        Description = description;

        var localHostOption = new Option<string>(
            name: "--local-host",
            description: description,
            getDefaultValue: () => "127.0.0.1");

        var localPortOption = new Option<int>(
            name: "--local-port",
            description: description,
            getDefaultValue: () => 80);

        var remoteUrlOption = new Option<string>(
            name: "--remote-url",
            description: description,
            getDefaultValue: () => "https://localtunnel.me/"); // todo: try bind to Uri

        var remoteDomainRequestOption = new Option<string>(
            name: "--remote-domain-request",
            description: description,
            getDefaultValue: () => "?new"); // todo: maybe we will use enum here (/green-cobra-733)

        Add(localHostOption);
        Add(localPortOption);
        Add(remoteUrlOption);
        Add(remoteDomainRequestOption);

        this.SetHandler<string, int, string, string>(
            async (localUrl, localPort, remoteUrl, remoteDomainRequest) =>
            {
                var localEndPoint = IPEndPoint.Parse($"{localUrl}:{localPort}");
                var proxyServerConfig = await GetProxyServerConfigurationAsync(remoteUrl, remoteDomainRequest);

                // log to console
                Console.WriteLine(localEndPoint.ToString());
                Console.WriteLine(proxyServerConfig.ToString());

                var proxyManager = new ProxyStreamManager(proxyServerConfig, localEndPoint);

                await proxyManager.RunProxyPoolAsync();
            },
            localHostOption,
            localPortOption,
            remoteUrlOption,
            remoteDomainRequestOption);
    }

    private async Task<ProxyServerConfiguration> GetProxyServerConfigurationAsync(string remoteUrl, string remoteDomainRequest)
    {
        HttpClient httpClient = new() {BaseAddress = new Uri(remoteUrl)};

        // todo: handle exception
        var proxyServerConfig = await httpClient.GetFromJsonAsync<ProxyServerConfiguration>(remoteDomainRequest);

        proxyServerConfig.IpEndPoint = await ResolveRemoteAddressAsync(proxyServerConfig);

        return proxyServerConfig;
    }

    private async Task<IPEndPoint> ResolveRemoteAddressAsync(ProxyServerConfiguration proxyServerConfig)
    {
        var ipAddress = (await Dns.GetHostAddressesAsync(
                proxyServerConfig.Url.DnsSafeHost,
                AddressFamily.InterNetwork))
            .First();

        return new IPEndPoint(ipAddress, proxyServerConfig.Port);
    }
}