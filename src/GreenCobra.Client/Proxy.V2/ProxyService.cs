using System.Diagnostics;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Client.Proxy.V2.Guards;

namespace GreenCobra.Client.Proxy.V2;

public class ProxyService : IProxyService
{
    private readonly ILoggerAdapter<ProxyService> _logger;
    //private readonly IProxyWatcher

    private ProxyConnection? _proxyConnection;

    public ProxyService(ILoggerAdapter<ProxyService> logger)
    {
        Guard.AgainstNull(logger);

        _logger = logger;
    }

    public async Task StartProxyAsync(ProxyConfiguration configuration, CancellationToken cancellationToken)
    {
        // todo: start watcher here
        _proxyConnection = new ProxyConnection(configuration.ClientEndPoint, configuration.ServerEndPoint);
        
        var proxyPool = SpawnTasks(configuration, cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(proxyPool);

            switch (completedTask)
            {
                case {Status: TaskStatus.RanToCompletion}:
                    var proxyResult = await completedTask;

                    // todo: logging
                    _logger.LogInformation(new TaskProxiedDataState
                    {
                        ServerEndPoint = configuration.ServerEndPoint,
                        ClientEndPoint = configuration.ClientEndPoint,
                        ProxyResult = proxyResult
                    });

                    break;
                case {Status: TaskStatus.Canceled}:
                case {Status: TaskStatus.Faulted}:
                    throw new TaskCanceledException(completedTask); // not the best way to handle, but suitable for now
                default:
                    throw new ArgumentOutOfRangeException(nameof(completedTask.Status),
                        "Task status doesn't define task completion");
            }

            proxyPool.Remove(completedTask);
            proxyPool.Add(_proxyConnection.ProxyAsync(cancellationToken));
        }

        // todo: await watcher task here
    }

    private List<Task<ProxyResult>> SpawnTasks(ProxyConfiguration proxyConfig, CancellationToken cancellationToken)
    {
        Debug.Assert(_proxyConnection is not null);

        var tasks = new List<Task<ProxyResult>>();
        for (var i = 0; i < proxyConfig.MaxConnections; i++)
        {
            var proxyTask = _proxyConnection.ProxyAsync(cancellationToken);
            tasks.Add(proxyTask);
        }

        return tasks;
    }
}