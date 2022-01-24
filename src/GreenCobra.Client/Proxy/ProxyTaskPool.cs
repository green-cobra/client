using GreenCobra.Client.Commands.Proxy.Configuration;

namespace GreenCobra.Client.Proxy;

public class ProxyTaskPool
{
    public readonly List<Task> ProxyTasks = new();
    public Task WatcherTask;

    private readonly ProxyConfiguration _proxyConfiguration;

    public ProxyTaskPool(ProxyConfiguration proxyConfiguration)
    {
        _proxyConfiguration = proxyConfiguration;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        WatcherTask = Task.Run(WatcherAction, cancellationToken);

        var proxyTask = Task.Run(_proxyFunc(_proxyConfiguration), cancellationToken);
        for (int i = 0; i < _proxyConfiguration.MaxConnections; i++)
        {
            ProxyTasks.Add(proxyTask);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(ProxyTasks);

            // todo: check for completed Task state
            //if (completedTask.IsCompleted)
            //{
                ProxyTasks.Remove(completedTask);

                ProxyTasks.Add(proxyTask);
            //}
        }
    }

    private Func<Task> _proxyFunc(ProxyConfiguration proxyConfiguration) =>
        async () =>
        {
            using var remoteProxyStream = 
                new ProxyStream(proxyConfiguration.ServerEndPoint, ProxyStreamType.Remote);
            using var localProxyStream = 
                new ProxyStream(proxyConfiguration.LocalApplicationEndPoint, ProxyStreamType.Local);

            Console.WriteLine($"    -  New Remote Connection {remoteProxyStream.Id}");
            Console.WriteLine($"    -  New Local Connection {localProxyStream.Id}");

            var remoteProxyTask = remoteProxyStream.CopyAsync(localProxyStream);
            var localProxyTask = localProxyStream.CopyAsync(remoteProxyStream);

            await Task.WhenAll(remoteProxyTask, localProxyTask);
        };

    private void WatcherAction()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Actively running at pool:{ProxyTasks.Count}");
            Console.ResetColor();

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}