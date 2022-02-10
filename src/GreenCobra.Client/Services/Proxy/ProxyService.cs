using System.Diagnostics;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;
using GreenCobra.Common;
using GreenCobra.Proxy;

namespace GreenCobra.Client.Services.Proxy;

public class ProxyService
{
    private readonly LoggerAdapter _logger;
    //private readonly IProxyWatcher

    private ProxyConnection? _proxyConnection;

    public ProxyService(LoggerAdapter logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }

    public async Task StartProxyAsync(ProxyConnectionOptions connectionOptions, CancellationToken cancellationToken)
    {
        // todo: start watcher here
        _proxyConnection = new ProxyConnection(connectionOptions.LocalEndPoint, connectionOptions.ServerEndPoint);
      
        var proxyPool = SpawnTasks(connectionOptions, cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(proxyPool);

            switch (completedTask)
            {
                case {Status: TaskStatus.RanToCompletion}:
                    var proxyResult = await completedTask;

                    _logger.Log(new TaskProxiedDataState(connectionOptions, proxyResult));

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

    private List<Task<ProxyResult>> SpawnTasks(ProxyConnectionOptions connectionOptions, CancellationToken cancellationToken)
    {
        Debug.Assert(_proxyConnection is not null);

        var tasks = new List<Task<ProxyResult>>();
        for (var i = 0; i < connectionOptions.ParallelDegree; i++)
        {
            var proxyTask = _proxyConnection.ProxyAsync(cancellationToken);
            tasks.Add(proxyTask);
        }

        return tasks;
    }
}