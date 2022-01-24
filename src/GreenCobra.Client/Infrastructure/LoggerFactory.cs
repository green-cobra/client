using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GreenCobra.Client.Infrastructure;

public static class LoggerFactory
{
    public static ILogger GetLogger()
    {
        using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                    options.SingleLine = true;
                    options.UseUtcTimestamp = true;
                });
            //.AddJsonConsole(options =>
            //    options.JsonWriterOptions = new JsonWriterOptions()
            //    {
            //        Indented = true
            //    });

            builder.SetMinimumLevel(LogLevel.Debug);
        });
        var logger = loggerFactory.CreateLogger<Program>();

        //using (logger.BeginScope("test scope"))
        //{
        //    logger.LogDebug(23, "message", "aa");

        //    logger.LogDebug("aaa");
        //}
        //logger.LogInformation("Info Log");
        //logger.LogWarning("Warning Log");
        //logger.LogError("Error Log");
        //logger.LogCritical("Critical Log");
    }
}