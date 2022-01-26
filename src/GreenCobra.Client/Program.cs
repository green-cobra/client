using System.CommandLine;
using GreenCobra.Client.Commands;

var debugParams = $"green-cobra proxy " +
                  $"--remote-domain-request green-cobra-7476 " +
                  $"--local-port 57679 ";

//var services = Startup.ConfigureServices();
//var serviceProvider = services.BuildServiceProvider();

//var bootstrap = serviceProvider.GetService<RootCommand>();


var bootstrap = new GreenCobraRootCommand();
await bootstrap.InvokeAsync(debugParams);