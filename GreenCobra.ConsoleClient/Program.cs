using System.CommandLine;
using GreenCobra.ConsoleClient;

//var debugParams = $"green-cobra --local-port 57678";

var bootstrap = new GreenCobraRootCommand();

await bootstrap.InvokeAsync(args);