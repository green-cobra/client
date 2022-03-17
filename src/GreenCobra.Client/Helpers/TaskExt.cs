namespace GreenCobra.Client.Helpers;

public static class TaskExt
{
    public static async Task<IEnumerable<T>> WhenAll<T>(params Task<T>[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            return await allTasks;
        }
        catch (Exception)
        {
            // ignored
        }

        throw allTasks.Exception ?? throw new AggregateException("This shouldn't happen");
    }
} 