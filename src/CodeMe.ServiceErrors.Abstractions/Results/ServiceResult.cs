using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CodeMe.ServiceErrors.Results;

/// <summary>
/// Represents the outcome of an operation that succeeds without a value or fails with a <see cref="ServiceError"/>.
/// </summary>
[Union]
public readonly struct ServiceResult : IEquatable<ServiceResult>, ServiceResult.IUnionMembers
{
    /// <summary>
    /// Determines whether two results are equal.
    /// </summary>
    public static bool operator ==(ServiceResult left, ServiceResult right) => Equals(left, right);

    /// <summary>
    /// Determines whether two results are not equal.
    /// </summary>
    public static bool operator !=(ServiceResult left, ServiceResult right) => !(left == right);

    /// <summary>
    /// Creates a failed result from a service error.
    /// </summary>
    /// <param name="error">The service error.</param>
    public static implicit operator ServiceResult(ServiceError error) => new(error);

    /// <summary>
    /// Creates a successful result from an <see cref="OkResult"/>.
    /// </summary>
    /// <param name="ok">The success marker.</param>
    public static implicit operator ServiceResult(OkResult ok) => new();

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="ServiceResult"/>.</returns>
    public static ServiceResult Ok() => new();

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The service error.</param>
    /// <returns>A failed <see cref="ServiceResult"/>.</returns>
    public static ServiceResult FromError(ServiceError error) => new(error);

    /// <summary>
    /// Creates a successful result containing a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The successful value.</param>
    /// <returns>A successful <see cref="ServiceResult{T}"/>.</returns>
    public static ServiceResult<T> Ok<T>(T value) => new(value);

    /// <summary>
    /// Creates a failed result with a specified value type.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="error">The service error.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/>.</returns>
    public static ServiceResult<T> FromError<T>(ServiceError error) => new(error);

    private readonly ServiceError? _error;

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    public ServiceResult()
    {
        _error = null;
    }

    /// <summary>
    /// Initializes a failed result.
    /// </summary>
    /// <param name="error">The service error.</param>
    public ServiceResult(ServiceError error)
    {
        _error = error;
    }

    /// <summary>
    /// Gets a value indicating whether this result contains a service error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => Error != null;

    /// <summary>
    /// Gets the service error when this result represents a failure.
    /// </summary>
    public ServiceError? Error => _error;

    bool IUnionMembers.HasValue => true;

    object IUnionMembers.Value => _error ?? (object)new OkResult();

    bool IUnionMembers.TryGetValue(out OkResult value)
    {
        value = default;
        return _error == null;
    }

    bool IUnionMembers.TryGetValue(out ServiceError value)
    {
        value = _error.GetValueOrDefault();
        return _error != null;
    }

    /// <summary>
    /// Determines whether this result is equal to another result.
    /// </summary>
    /// <param name="other">The other result to compare.</param>
    /// <returns><see langword="true"/> if the results are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(ServiceResult other) =>
        _error switch
        {
            { } error when other._error is { } otherError => error.Equals(otherError),
            null when other._error is null => true,
            _ => false
        };

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ServiceResult result && Equals(result);

    /// <summary>
    /// Returns the hash code for this result.
    /// </summary>
    /// <returns>The hash code for this result.</returns>
    public override int GetHashCode() => _error?.GetHashCode() ?? 0;

    /// <summary>
    /// Defines the union members of <see cref="ServiceResult"/>.
    /// </summary>
    public interface IUnionMembers
    {
        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="ok">The success marker.</param>
        /// <returns>A successful <see cref="ServiceResult"/>.</returns>
        public static ServiceResult Create(OkResult ok) => new();

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error">The service error.</param>
        /// <returns>A failed <see cref="ServiceResult"/>.</returns>
        public static ServiceResult Create(ServiceError error) => new(error);

        /// <summary>
        /// Gets the current union value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets a value indicating whether the union has a value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Attempts to get the success marker.
        /// </summary>
        /// <param name="value">The success marker when this result is successful.</param>
        /// <returns><see langword="true"/> if this result is successful; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValue(out OkResult value);

        /// <summary>
        /// Attempts to get the service error.
        /// </summary>
        /// <param name="value">The service error when this result is failed.</param>
        /// <returns><see langword="true"/> if this result is failed; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValue(out ServiceError value);
    }
}
