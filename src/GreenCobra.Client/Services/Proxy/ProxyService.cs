using System.Net;
using GreenCobra.Common;
using GreenCobra.Proxy;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Services.Proxy;

public class ProxyService
{
    private readonly ILogger<ProxyService> _logger;
    //private readonly IProxyWatcher

    public ProxyService(ILogger<ProxyService> logger)
    {
        Guard.AgainstNull(logger);
        _logger = logger;
    }

    public async Task StartProxyAsync(EndPoint from, EndPoint to, int parallelDegree = 1, CancellationToken cancellationToken = default)
    {
        // todo: start watcher here
        var proxyConnection = new ProxyConnection(from, to);
        
        _logger.LogDebug(Resources.Logs.ProxyConnectionSetupDone, from, to);
      
        var proxyPool = SpawnTasks(proxyConnection, parallelDegree, cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug(Resources.Logs.ProxyParallelConnectionStarted, parallelDegree);
            
            var completedTask = await Task.WhenAny(proxyPool);

            _logger.LogDebug(Resources.Logs.ProxyTaskCompleted, completedTask.Status);
            
            switch (completedTask)
            {
                case {Status: TaskStatus.RanToCompletion}:
                    var proxyResult = await completedTask;

                    // string ByteToString(byte[] bytes)
                        // => Encoding.UTF8.GetString(bytes);
                    // todo: add logging

                    break;
                case {Status: TaskStatus.Canceled}:
                case {Status: TaskStatus.Faulted}:
                    throw new TaskCanceledException(completedTask); // not the best way to handle, but suitable for now
                default:
                    throw new ArgumentOutOfRangeException(nameof(completedTask.Status),
                        "Task status doesn't define task completion");
            }

            proxyPool.Remove(completedTask);
            proxyPool.Add(proxyConnection.ProxyAsync(cancellationToken));
        }

        // todo: await watcher task here
    }

    private List<Task<ProxyResult>> SpawnTasks(ProxyConnection connection, int parallelDegree, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<ProxyResult>>();
        for (var i = 0; i < parallelDegree; i++)
        {
            var proxyTask = connection.ProxyAsync(cancellationToken);
            tasks.Add(proxyTask);
        }

        return tasks;
    }
}