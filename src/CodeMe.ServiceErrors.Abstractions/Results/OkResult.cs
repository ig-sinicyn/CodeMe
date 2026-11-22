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
    /// <param name="other">The other instance to compare.</param>
    /// <returns><see langword="true"/> because all <see cref="OkResult"/> instances are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(OkResult other) => true;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is OkResult;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>Always zero.</returns>
    public override int GetHashCode() => 0;
}
