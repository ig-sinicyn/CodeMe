using System.Diagnostics.CodeAnalysis;
using System.Text;
using CodeMe.ServiceErrors.Polyfills;

namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error group (RFC 9457-compatible problem type).
/// </summary>
public sealed class ErrorGroupUri : IEquatable<ErrorGroupUri>
{
    /// <summary>
    /// Error group separator.
    /// </summary>
    public const char GroupSeparator = '/';

    /// <summary>
    /// Error group string comparison.
    /// </summary>
    public const StringComparison UriPartComparison = StringComparison.OrdinalIgnoreCase;


    /// <summary>
    /// Determines whether two error groups are equal.
    /// </summary>
    public static bool operator ==(ErrorGroupUri? left, ErrorGroupUri? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Indicates whether two error groups are not equal.
    /// </summary>
    public static bool operator !=(ErrorGroupUri? left, ErrorGroupUri? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Creates error group.
    /// </summary>
    public static ErrorGroupUri Create(string scheme, string application, string? category = null)
    {
        if (string.IsNullOrEmpty(category)) return ValidateAndCreate(scheme, application, null);

        var formattedCategory = new StringBuilder();
        AppendCategory(formattedCategory, category);

        return ValidateAndCreate(scheme, application, formattedCategory.ToString());
    }

    /// <summary>
    /// Creates error group.
    /// </summary>
    public static ErrorGroupUri Create(string scheme, string application, params ReadOnlySpan<string> categories)
    {
        if (categories.IsEmpty) return ValidateAndCreate(scheme, application, null);

        var formattedCategory = new StringBuilder();
        foreach (var category in categories) AppendCategory(formattedCategory, category);

        return ValidateAndCreate(scheme, application, formattedCategory.ToString());
    }

    private static ErrorGroupUri Create(string scheme, string application, string category,
        params ReadOnlySpan<string> subCategories)
    {
        var formattedCategory = new StringBuilder();
        AppendCategory(formattedCategory, category);
        foreach (var subCategory in subCategories) AppendCategory(formattedCategory, subCategory);

        return ValidateAndCreate(scheme, application, formattedCategory.ToString());
    }

    /// <summary>
    /// Creates error group.
    /// </summary>
    public static ErrorGroupUri Create(string scheme, string application, IEnumerable<string> categories)
    {
        var formattedCategory = new StringBuilder();
        foreach (var category in categories) AppendCategory(formattedCategory, category);

        return ValidateAndCreate(scheme, application, formattedCategory.ToString());
    }

    private static void AppendCategory(StringBuilder fullCategory, string category)
    {
        var categorySpan = category.AsSpan();
        var segments = categorySpan.SplitToSpans(GroupSeparator);
        foreach (var segment in segments)
        {
            if (segment.Length == 0) continue;

            if (fullCategory.Length > 0) fullCategory.Append(GroupSeparator);

            fullCategory.Append(segment);
        }
    }

    /// <summary>
    /// Creates error group from <see cref="Uri"/>.
    /// </summary>
    public static ErrorGroupUri Parse(Uri errorUri)
    {
        ArgumentNullExceptionEx.ThrowIfNull(errorUri);

        if (!errorUri.IsAbsoluteUri)
            throw new ArgumentException(
                $"Error group URI {errorUri} must be an absolute URI",
                nameof(errorUri));

        if (errorUri.Host != errorUri.IdnHost)
            throw new ArgumentException(
                $"Error group URI {errorUri} should be in IDN format",
                nameof(errorUri));

        var category = errorUri.AbsolutePath;
        if (!category.EndsWith(GroupSeparator))
            throw new ArgumentException(
                $"Error group URI {errorUri} must end with '{GroupSeparator}'",
                nameof(errorUri));

        return Create(errorUri.Scheme, errorUri.Host, Uri.UnescapeDataString(category));
    }

    /// <summary>
    /// Creates error group from URI string.
    /// </summary>
    public static ErrorGroupUri Parse(string errorUri)
    {
        try
        {
            return Parse(new Uri(errorUri));
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException($"Invalid uri {errorUri} format", nameof(errorUri), ex);
        }
    }

    /// <summary>
    /// Creates error group from <see cref="Uri"/>.
    /// </summary>
    public static bool TryParse(Uri errorUri, [MaybeNullWhen(false)] out ErrorGroupUri result)
    {
        if (errorUri == null!
            || !errorUri.IsAbsoluteUri
            || errorUri.Host != errorUri.IdnHost
            || !errorUri.AbsolutePath.EndsWith(GroupSeparator))
        {
            result = null;
            return false;
        }

        var category = Uri.UnescapeDataString(errorUri.AbsolutePath);
        result = Create(errorUri.Scheme, errorUri.Host, category);
        return true;
    }

    /// <summary>
    /// Creates error group from URI string.
    /// </summary>
    public static bool TryParse(
        [NotNullWhen(true)] string? errorUri,
        [MaybeNullWhen(false)] out ErrorGroupUri result)
    {
        if (!Uri.TryCreate(errorUri, UriKind.Absolute, out var uri))
        {
            result = null;
            return false;
        }

        return TryParse(uri, out result);
    }

    private static string? GetParentCategory(string? category)
    {
        return category.AsSpan().Trim(GroupSeparator) switch
        {
            { IsEmpty: true } => null,
            var x when x.LastIndexOf(GroupSeparator) is var index && index >= 0 => x[..index].ToString(),
            _ => null
        };
    }

    private static ErrorGroupUri ValidateAndCreate(string scheme, string application, string? category)
    {
        ArgumentExceptionEx.ThrowIfNullOrEmpty(scheme);
        if (!Uri.CheckSchemeName(scheme)) throw new ArgumentException($"Invalid scheme {scheme}", nameof(scheme));

        ArgumentExceptionEx.ThrowIfNullOrEmpty(application);
        if (application != Uri.EscapeDataString(application))
            throw new ArgumentException(
                $"Application name {application} should be valid host name",
                nameof(application));

        if (string.IsNullOrEmpty(category)) category = null;

        return new ErrorGroupUri(scheme, application, category);
    }

    private ErrorGroupUri(string scheme, string application, string? category)
    {
        Scheme = scheme;
        Application = application;
        Category = category;
    }

    /// <summary>
    /// Error uri scheme.
    /// </summary>
    public string Scheme { get; }

    /// <summary>
    /// Application name.
    /// </summary>
    public string Application { get; }

    /// <summary>
    /// Error category.
    /// </summary>
    public string? Category { get; }

    /// <summary>
    /// URI representation of error group.
    /// </summary>
    public Uri Uri => field ??= new UriBuilder(Scheme, Application)
    {
        Path = Category + GroupSeparator
    }.Uri;

    /// <summary>
    /// Checks if the error group has no category (is a root group).
    /// </summary>
    public bool IsApplicationGroup => Category == null;

    /// <summary>
    /// Returns the root error group.
    /// </summary>
    public ErrorGroupUri ApplicationGroup =>
        Category == null ? this : new ErrorGroupUri(Scheme, Application, null);

    /// <summary>
    /// Returns parent error group, or <c>null</c> for the root group.
    /// </summary>
    public ErrorGroupUri? ParentGroup =>
        IsApplicationGroup ? null : new ErrorGroupUri(Scheme, Application, GetParentCategory(Category));

    /// <summary>
    /// Returns subgroup.
    /// </summary>
    public ErrorGroupUri SubGroup(string subCategory)
    {
        return Create(Scheme, Application, Category ?? "", subCategory);
    }

    /// <summary>
    /// Returns subgroup.
    /// </summary>
    public ErrorGroupUri SubGroup(params ReadOnlySpan<string> subCategories)
    {
        return Create(Scheme, Application, Category ?? "", subCategories);
    }

    /// <summary>
    /// Returns subgroup.
    /// </summary>
    public ErrorGroupUri SubGroup(IEnumerable<string> subCategories)
    {
        return Create(Scheme, Application, subCategories.Prepend(Category ?? ""));
    }

    /// <summary>
    /// Checks if error group contains the given subgroup.
    /// </summary>
    public bool Contains(ErrorGroupUri other)
    {
        if (!string.Equals(Scheme, other.Scheme, UriPartComparison)) return false;

        if (!string.Equals(Application, other.Application, UriPartComparison)) return false;

        if (Category == null) return true;

        if (other.Category == null) return false;

        return other.Category.StartsWith(Category, UriPartComparison)
            && (other.Category.Length == Category.Length
                || other.Category[Category.Length] == GroupSeparator);
    }

    /// <summary>
    /// Checks if error group contains the given error uri.
    /// </summary>
    public bool Contains(ErrorUri errorUri)
    {
        return Contains(errorUri.Group);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Uri.ToString();
    }

    /// <inheritdoc/>
    public bool Equals(ErrorGroupUri? other)
    {
        return other switch
        {
            null => false,
            _ when ReferenceEquals(this, other) => true,
            _ when string.Equals(Scheme, other.Scheme, UriPartComparison)
                && string.Equals(Application, other.Application, UriPartComparison)
                && string.Equals(Category, other.Category, UriPartComparison) => true,
            _ => false
        };
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ErrorGroupUri);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Scheme),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Application),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Category ?? ""));
    }
}