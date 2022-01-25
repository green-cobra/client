// this is developments for possible using of compile time logging

//using GreenCobra.Client.Commands.Proxy.Configuration;
//using GreenCobra.Client.Commands.Proxy.Handlers;
//using Microsoft.Extensions.Logging;

//namespace GreenCobra.Client.Commands.Proxy.Loggers;

//public static partial class ProxyCommandHandlerLogs
//{
//    [LoggerMessage(
//        EventId = 0,
//        Level = LogLevel.Critical,
//        Message = @"Could not open socket to {hostName}")]
//    public static partial void CouldNotOpenSocket(this
//        ILogger<ProxyCommandHandler> logger, string hostName);

//    [LoggerMessage(
//        EventId = 0,
//        Level = LogLevel.Debug,
//        Message = @"Proxy command invoked with the following params: {proxyParams}")]
//    public static partial void ProxyParams(this
//        ILogger<ProxyCommandHandler> logger, ProxyCommandParams proxyParams);    

//    [LoggerMessage(
//        EventId = 0,
//        Level = LogLevel.Debug,
//        Message = @"Proxy command invoked with the following params: {args}")]
//    public static partial void ProxyParamsSimple(this
//        ILogger<ProxyCommandHandler> logger, params object[] args);
//}

//public enum LogEvent
//{
//    ProxyCommandHandler_Entrance = 11
//}