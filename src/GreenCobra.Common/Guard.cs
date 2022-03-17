using System.Runtime.CompilerServices;

namespace GreenCobra.Common;

public static class Guard
{
    public static void AgainstNull<T>(T valueToCheck, 
        [CallerArgumentExpression("valueToCheck")] string message = "")
    {
        if (valueToCheck is null)
            throw new ArgumentNullException(message);
    }

    public static void AgainstNullOrEmpty(string valueToCheck,
        [CallerArgumentExpression("valueToCheck")]
        string message = "")
    {
        if (string.IsNullOrEmpty(valueToCheck))
            throw new ArgumentOutOfRangeException(message);
    }
    
    public static void Satisfy(bool condition, 
        [CallerArgumentExpression("condition")] string message = "")
    {
        if (!condition)
            throw new ArgumentOutOfRangeException(message);
    }
}