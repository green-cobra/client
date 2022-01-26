//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Console;

//namespace GreenCobra.Client.Logging;

//public class CommandLoggerFactory : IDisposable
//{
//    private static readonly ILoggerFactory LoggerFactoryInstance;

//    private CommandLoggerFactory() { }

//    static CommandLoggerFactory()
//    {
//        // todo: use Service Collection around whole application
//        // todo: move to config file
//        LoggerFactoryInstance = LoggerFactory.Create(builder =>
//        {
//            builder
//                //.AddJsonConsole()
//                .AddSimpleConsole(options =>
//                {
//                    options.IncludeScopes = true;
//                    options.ColorBehavior = LoggerColorBehavior.Enabled;
//                    options.SingleLine = true;
//                    options.UseUtcTimestamp = true;
//                });

//            builder.SetMinimumLevel(LogLevel.Debug);
//        });
//    }

//    public static ILogger<T> GetLogger<T>() => LoggerFactoryInstance.CreateLogger<T>();

//    public void Dispose()
//    {
//        LoggerFactoryInstance.Dispose();
//    }
//}