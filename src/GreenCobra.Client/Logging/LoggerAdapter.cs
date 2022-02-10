using System.CommandLine;
using GreenCobra.Client.Logging.States;

namespace GreenCobra.Client.Logging;

public class LoggerAdapter
{
    private readonly IConsole _console;

    public LoggerAdapter(IConsole console)
    {
        _console = console;
    }

    public void Log(ILoggerState state)
    {
        _console.WriteLine(state.Format());
    }
}