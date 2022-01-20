namespace GreenCobra.Client;

public class ProxyTaskPool
{
    public readonly List<Task> ProxyTasks = new();
    public Task WatcherTask;

    private readonly int _parallelTaskCount;
    private readonly Func<Task> _templateAction;

    public ProxyTaskPool(
        int parallelTaskCount, 
        Func<Task> templateAction)
    {
        _parallelTaskCount = parallelTaskCount;
        _templateAction = templateAction;
    }

    public async Task RunAsync(CancellationToken token)
    {
        WatcherTask = Task.Run(WatcherAction);

        for (int i = 0; i < _parallelTaskCount; i++)
        {
            ProxyTasks.Add(Task.Run(_templateAction));
        }

        while (!token.IsCancellationRequested)
        {
            var completedTask = await Task.WhenAny(ProxyTasks);

            // todo: check for completed Task state

            //if (completedTask.IsCompleted)
            //{
                ProxyTasks.Remove(completedTask);

                ProxyTasks.Add(Task.Run(_templateAction));
            //}
        }
    }

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