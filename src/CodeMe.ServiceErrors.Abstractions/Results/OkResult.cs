namespace CodeMe.ServiceErrors.Results;

/// <summary>
/// Represents a successful result that has no value.
/// </summary>
public readonly struct OkResult : IEquatable<OkResult>
{
    /// <summary>
    /// Determines whether this instance is equal to another <see cref="OkResult"/>.
    /// All <see cref="OkResult"/> instances are equal.
    /// </summary>
    public bool Equals(OkResult other) => true;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is OkResult;

    /// <inheritdoc/>
    public override int GetHashCode() => 0;
}
