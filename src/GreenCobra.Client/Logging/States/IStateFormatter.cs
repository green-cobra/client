namespace GreenCobra.Client.Logging.States;

public interface IStateFormatter<in T> where T : IState
{
    public Func<T, Exception?, string>? Formatter { get; }
}