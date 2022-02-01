namespace GreenCobra.Client.Logging;

public enum LoggingEventId
{
    // config events
    Proxy_ConfigurationDone = 10,

    // proxying events
    DataProxied = 1000,

    // other events
    PoolWatcher_GotPoolStatus = 5000,
}