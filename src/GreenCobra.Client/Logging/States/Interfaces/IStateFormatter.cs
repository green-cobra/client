namespace GreenCobra.Client.Logging.States.Interfaces;

public interface IStateFormatter<in T> where T : IState
{
    public Func<T, Exception?, string> Formatter { get; }
}