namespace GreenCobra.Client.Logging;

public enum LoggingEventId
{
    // config events
    GotServerConfiguration = 10,

    // proxying events
    DataProxied = 1000,

    // other events
    PoolWatcher_GotPoolStatus = 5000,
}