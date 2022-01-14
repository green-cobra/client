using System.Net;

namespace GreenCobra.Client.Console;

public class ProxyStreamManager
{
    private readonly int _connectionLimit;
    private readonly IPEndPoint _serverProxyEndPoint;

    private readonly IPEndPoint _localEndPoint;

    private readonly List<Task> _activeProxyTasks = new();

    public ProxyStreamManager(
        ProxyServerConfiguration serverConfiguration, 
        IPEndPoint localEndPoint)
    {
        _localEndPoint = localEndPoint;

        _connectionLimit = serverConfiguration.ConnectionLimit;
        _serverProxyEndPoint = serverConfiguration.IpEndPoint;
    }

    // todo: add cancellation token
    public async Task RunProxyPoolAsync()
    {
        for (int i = 0; i < _connectionLimit; i++)
        {
            var proxyTask = StartProxyAsync();
                //.ContinueWith((result => _activeProxyTasks.Remove(result));
            _activeProxyTasks.Add(proxyTask);

            System.Console.WriteLine($"Task Created {proxyTask.Id}");
        }

        while (true) // while (cancellationToken)
        {
            /* todo: when connection unsuccessful no error appears
            so we need to check was task completed with success or failure */
            var completedTask = await Task.WhenAny(_activeProxyTasks);

            System.Console.WriteLine($"Task Completed {completedTask.Id}");

            _activeProxyTasks.Remove(completedTask);
            var proxyTask = StartProxyAsync();
            _activeProxyTasks.Add(proxyTask);

            System.Console.WriteLine($"  -  Task Created {proxyTask.Id}");
        }
    }

    private async Task StartProxyAsync()
    {
        using var remoteProxyStream = new ProxyStream(_serverProxyEndPoint, ProxyStreamType.Remote);
        using var localProxyStream = new ProxyStream(_localEndPoint, ProxyStreamType.Local);

        System.Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
        System.Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

        var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
        var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

        await Task.WhenAll(remoteProxyTask, localProxyTask);
    }
}

