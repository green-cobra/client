using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Proxy;

public interface IProxyTaskPool
{
    Task RunAsync(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken);
}

public class ProxyTaskPool : IProxyTaskPool
{
    public readonly List<Task> ProxyTasks = new();
    public Task WatcherTask;

    //private ProxyConfiguration _proxyConfiguration;
    private readonly ILogger<ProxyTaskPool> _logger;

    public ProxyTaskPool(ILogger<ProxyTaskPool> logger)
    {
        //_proxyConfiguration = proxyConfiguration;
        _logger = logger;
    }

    public async Task RunAsync(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Proxy pool config: {proxyConfiguration}");

        WatcherTask = Task.Run(_watcherAction(cancellationToken), cancellationToken);

        for (int i = 0; i < proxyConfiguration.MaxConnections; i++)
        {
            StartNewProxyTask(proxyConfiguration, cancellationToken);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(ProxyTasks);


            _logger.LogDebug($"Task {completedTask.Id}; Status: {completedTask.Status}");
            // todo: check for completed Task state
            //if (completedTask.IsCompleted)
            //{
            ProxyTasks.Remove(completedTask);
            StartNewProxyTask(proxyConfiguration, cancellationToken);
            //}
        }

        await WatcherTask;
    }

    private void StartNewProxyTask(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken)
    {
        var proxyTask = Task.Run(async () =>
        {
            var state = new ProxyTaskState(Task.CurrentId.Value);
            using var scope = _logger.BeginScope(state);
            _logger.LogDebug($"Proxy task started, id {state.Id}");

            var connection = new ProxyConnection(proxyConfiguration.ServerEndPoint,
                proxyConfiguration.LocalApplicationEndPoint);

            _logger.LogDebug($"Proxy connection initialized; conn_id {connection.Id}");

            await connection.ProxyAsync(cancellationToken);

            _logger.LogDebug($"Task {state.Id}; Status: task going for cancellation");
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