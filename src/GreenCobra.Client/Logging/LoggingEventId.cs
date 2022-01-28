namespace GreenCobra.Client.Logging;

public enum LoggingEventId
{
    // config events
    Proxy_ConfigurationDone = 10,

    // proxying events
    ProxyTaskCompleted = 100,
    ProxyStream_DataProxied = 1000,

    // other events
    PoolWatcher_GotPoolStatus = 5000
}