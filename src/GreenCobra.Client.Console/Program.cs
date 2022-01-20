using System.CommandLine;
using GreenCobra.Client.Console.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

var debugParams = $"green-cobra proxy " +
                  $"--remote-domain-request green-cobra-7476 " +
                  $"--local-port 57679 ";

using var loggerFactory = LoggerFactory.Create(builder =>
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
//ILogger logger = loggerFactory.CreateLogger<Program>();
//var logger = loggerFactory.CreateLogger<Program>();

//using (logger.BeginScope("test scope"))
//{
//    logger.LogDebug(23, "message", "aa");

//    logger.LogDebug("aaa");
//}
//logger.LogInformation("Info Log");
//logger.LogWarning("Warning Log");
//logger.LogError("Error Log");
//logger.LogCritical("Critical Log");

var bootstrap = new GreenCobraRootCommand();

await bootstrap.InvokeAsync(debugParams);