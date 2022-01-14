using System.CommandLine;
using GreenCobra.Client.Console;

var debugParams = $"green-cobra --local-port 57678";

var bootstrap = new GreenCobraRootCommand();

await bootstrap.InvokeAsync(debugParams /*args*/);