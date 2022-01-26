using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Proxy;

public class ProxyTaskPool : IProxyTaskPool
{
    private readonly ILogger<ProxyTaskPool> _logger;

    public ProxyTaskPool(ILogger<ProxyTaskPool> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Proxy pool config: {proxyConfiguration}");

        var proxyPool = SpawnTasks(proxyConfiguration, cancellationToken);
        var watcherTask = StartWatcher(proxyPool, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(proxyPool);
            // todo: check completed task status
            proxyPool.Remove(completedTask);
            proxyPool.Add(StartProxy(proxyConfiguration, cancellationToken));
        }

        await watcherTask;
    }

    private List<Task> SpawnTasks(ProxyConfiguration proxyConfig, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        for (var i = 0; i < proxyConfig.MaxConnections; i++)
        {
            tasks.Add(StartProxy(proxyConfig, cancellationToken));
        }

        return tasks;
    }

    private Task StartProxy(ProxyConfiguration proxyConfig, CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var state = new ProxyTaskState(Task.CurrentId.Value);
            using var scope = _logger.BeginScope(state);

            var connection = new ProxyConnection(proxyConfig.ServerEndPoint,
                proxyConfig.LocalApplicationEndPoint);

            _logger.LogDebug($"Proxy started; ConnectionId: {connection.Id}");

            await connection.ProxyAsync(cancellationToken);

            _logger.LogDebug($"Proxy completed; ConnectionId: {connection.Id}");

        }, cancellationToken);
    }

    private Task StartWatcher(List<Task> pool, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Actively running at pool:{pool.Count}");

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }, cancellationToken);
    }
}