using System.Text;

namespace CodeMe.ServiceErrors.Serializable;

internal static class ErrorUriConvert
{
    public static string ExceptionTypeToErrorCode(Type exceptionType)
    {
        if (!exceptionType.IsAssignableTo(typeof(Exception)))
            throw new ArgumentException($"The type {exceptionType} is not assignable to {typeof(Exception)}");

        return exceptionType.Name.TrimLast("Exception").ToLowerWithSeparator("-");
    }

    private static ReadOnlySpan<char> TrimLast(
        this ReadOnlySpan<char> value,
        string suffix)
    {
        ArgumentNullException.ThrowIfNull(suffix);

        return !value.EndsWith(suffix, StringComparison.Ordinal)
            ? value
            : value[..^suffix.Length];
    }

    private static string ToLowerWithSeparator(this ReadOnlySpan<char> value, string separator)
    {
        var result = new StringBuilder(value.Length);

        // HACK: prevent start underscore
        var prevIsUpper = true;

        foreach (var c in value)
            if (char.IsUpper(c))
            {
                if (!prevIsUpper) result.Append(separator);

                result.Append(char.ToLowerInvariant(c));
                prevIsUpper = true;
            }
            else
            {
                result.Append(c);
                prevIsUpper = false;
            }

        return result.ToString();
    }
}