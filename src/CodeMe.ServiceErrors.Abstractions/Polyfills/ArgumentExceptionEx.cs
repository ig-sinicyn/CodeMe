using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CodeMe.ServiceErrors.Polyfills;

internal static class ArgumentExceptionEx
{
    public static void ThrowIfNullOrEmpty([NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))]
        string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
            ThrowNullOrEmptyException(argument, paramName);
    }

    [DoesNotReturn]
    private static void ThrowNullOrEmptyException(string? argument, string? paramName)
    {
        ArgumentNullExceptionEx.ThrowIfNull(argument, paramName);
        throw new ArgumentException("Value cannot be an empty string.", paramName);
    }
}