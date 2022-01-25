using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Proxy;

public class ProxyTaskPool
{
    public readonly List<Task> ProxyTasks = new();
    public Task WatcherTask;

    private readonly ProxyConfiguration _proxyConfiguration;
    private readonly ILogger<ProxyTaskPool> _logger;

    public ProxyTaskPool(ProxyConfiguration proxyConfiguration)
    {
        _proxyConfiguration = proxyConfiguration;
        _logger = CommandLoggerFactory.GetLogger<ProxyTaskPool>();

        _logger.LogDebug($"Proxy pool config: {proxyConfiguration}");
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        WatcherTask = Task.Run(_watcherAction(cancellationToken), cancellationToken);

        for (int i = 0; i < _proxyConfiguration.MaxConnections; i++)
        {
            StartNewProxyTask(cancellationToken);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(ProxyTasks);

            // todo: check for completed Task state
            //if (completedTask.IsCompleted)
            //{
            ProxyTasks.Remove(completedTask);
            StartNewProxyTask(cancellationToken);
            //}
        }

        await WatcherTask;
    }

    private void StartNewProxyTask(CancellationToken cancellationToken)
    {
        var proxyTask = Task.Run(async () =>
        {
            var state = new ProxyTaskState(Task.CurrentId.Value);
            using var scope = _logger.BeginScope(state);
            _logger.LogDebug($"Proxy task started, id {state.Id}");

            var connection = new ProxyConnection(_proxyConfiguration.ServerEndPoint,
                _proxyConfiguration.LocalApplicationEndPoint);

            _logger.LogDebug($"Proxy connection initialized; conn_id {connection.Id}");

            await connection.ProxyAsync();
        }, cancellationToken);
        
        ProxyTasks.Add(proxyTask);
    }

    private Action _watcherAction(CancellationToken ct) =>
        () =>
        {
            while (!ct.IsCancellationRequested)
            {
                _logger.LogDebug($"Actively running at pool:{ProxyTasks.Count}");

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        };
}