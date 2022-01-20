using System.Net;
using GreenCobra.ConsoleClient.Configuration;

namespace GreenCobra.ConsoleClient.ProxyStream;

public class ProxyStreamManager
{
    private readonly ProxyTaskPool _taskPool;

    public ProxyStreamManager(
        ProxyServerConfiguration serverConfiguration, 
        IPEndPoint localEndPoint)
    {
        _taskPool = new ProxyTaskPool(
            serverConfiguration.ConnectionLimit,
            ProxyFunc(serverConfiguration, localEndPoint));
    }

    private static Func<Task> ProxyFunc(ProxyServerConfiguration serverConfiguration, IPEndPoint localEndPoint) => 
        async () =>
        {
            using var remoteProxyStream = new ProxyStream(localEndPoint, ProxyStreamType.Remote);
            using var localProxyStream = new ProxyStream(serverConfiguration.IpEndPoint, ProxyStreamType.Local);

            Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
            Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

            var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
            var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

            await Task.WhenAll(remoteProxyTask, localProxyTask);
        };

    public async Task RunProxyPoolAsync()
    {
        await _taskPool.RunAsync(CancellationToken.None);
    }
}

