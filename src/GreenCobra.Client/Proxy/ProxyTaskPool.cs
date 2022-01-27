using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Proxy;

public class ProxyTaskPool : IProxyTaskPool
{
    private readonly ILoggerAdapter<ProxyTaskPool, string> _logger;
    private readonly ILoggerAdapter<ProxyStream, byte[]> _streamLogger;

    public ProxyTaskPool(ILoggerAdapter<ProxyTaskPool, string> logger, 
        ILoggerAdapter<ProxyStream, byte[]> proxyLogger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _streamLogger = proxyLogger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    public async Task RunAsync(ProxyConfiguration proxyConfiguration, CancellationToken cancellationToken)
    {
        //_logger.LogDebug($"Proxy pool config: {proxyConfiguration}");
        _logger.LogInformation(EventIds.RetrievedProxyPoolConfig, $"Proxy pool config: {proxyConfiguration}");

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
            //var state = new ProxyTaskState(Task.CurrentId.Value, "This is Task");
            //using var scope = _logger.BeginScope(state);

            using var clientStream = new ProxyStream(proxyConfig.LocalApplicationEndPoint, _streamLogger);
            using var serverStream = new ProxyStream(proxyConfig.ServerEndPoint, _streamLogger);

            var serverToClient = serverStream.CopyAsync(clientStream, cancellationToken);
            var clientToServer = clientStream.CopyAsync(serverStream, cancellationToken);

            await Task.WhenAll(serverToClient, clientToServer);

            //_logger.LogDebug($"Proxy completed; TaskId: {Task.CurrentId.Value}");
            _logger.LogInformation(EventIds.ProxyTaskCompleted, $"Proxy completed; TaskId: {Task.CurrentId.Value}");
        }, cancellationToken);
    }

    private Task StartWatcher(List<Task> pool, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //_logger.LogDebug($"Actively running at pool:{pool.Count}");
                _logger.LogInformation(EventIds.PoolWatcher_GotPoolStatus, $"Actively running at pool:{pool.Count}");

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }, cancellationToken);
    }
}