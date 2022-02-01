using System.Runtime.CompilerServices;

namespace GreenCobra.Client.Helpers;

public static class Guard
{
    public static void AgainstNull<T>(T valueToCheck, 
        [CallerArgumentExpression("valueToCheck")] string message = "")
    {
        if (valueToCheck == null)
            throw new ArgumentNullException(message);
    }
}