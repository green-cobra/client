using System.CommandLine.Binding;

namespace GreenCobra.Client.Infrastructure;

public interface ICommandBinder<out T>
{
    public T BindParametersFromContext(BindingContext bindingContext);
}