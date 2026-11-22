using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CodeMe.ServiceErrors.Results;

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/> or fails with a <see cref="ServiceError"/>.
/// </summary>
[Union]
public readonly struct ServiceResult<T> : IEquatable<ServiceResult<T>>, ServiceResult<T>.IUnionMembers
{
    /// <summary>
    /// Determines whether two results are equal.
    /// </summary>
    public static bool operator ==(ServiceResult<T> left, ServiceResult<T> right) => Equals(left, right);

    /// <summary>
    /// Determines whether two results are not equal.
    /// </summary>
    public static bool operator !=(ServiceResult<T> left, ServiceResult<T> right) => !(left == right);

    /// <summary>
    /// Creates a successful result from a value.
    /// </summary>
    public static implicit operator ServiceResult<T>(T value) => new(value);

    /// <summary>
    /// Creates a failed result from a service error.
    /// </summary>
    public static implicit operator ServiceResult<T>(ServiceError error) => new(error);

    /// <summary>
    /// Converts a typed result to an untyped service result.
    /// </summary>
    public static implicit operator ServiceResult(ServiceResult<T> result) =>
        result is ServiceError error ? error : new OkResult();

    private readonly T _value;
    private readonly ServiceError? _error;

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    public ServiceResult(T value)
    {
        _value = value;
        _error = null;
    }

    /// <summary>
    /// Initializes a failed result.
    /// </summary>
    public ServiceResult(ServiceError error)
    {
        _value = default!;
        _error = error;
    }

    /// <summary>
    /// Gets a value indicating whether this result contains a service error.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => _error != null;

    /// <summary>
    /// Gets the successful value.
    /// </summary>
    public T Value => _error == null
        ? _value
        : throw new ServiceException(_error.Value);

    /// <summary>
    /// Gets the service error when this result represents a failure.
    /// </summary>
    public ServiceError? Error => _error;

    bool IUnionMembers.HasValue => _error != null || _value != null;

    object? IUnionMembers.Value => _error ?? (object?)_value;

    bool IUnionMembers.TryGetValue(out T value)
    {
        value = _value;
        return _error == null && _value != null;
    }

    bool IUnionMembers.TryGetValue(out ServiceError value)
    {
        value = _error.GetValueOrDefault();
        return _error != null;
    }

    /// <summary>
    /// Determines whether this result is equal to another result.
    /// </summary>
    public bool Equals(ServiceResult<T> other) =>
        _error switch
        {
            { } error when other._error is { } otherError => error.Equals(otherError),
            null when other._error is null => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ServiceResult<T> x && Equals(x);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        _error?.GetHashCode()
        ?? EqualityComparer<T>.Default.GetHashCode(_value);

    /// <summary>
    /// Defines the union members of <see cref="ServiceResult{T}"/>.
    /// </summary>
    public interface IUnionMembers
    {
        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static ServiceResult<T> Create(T value) => new(value);

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        public static ServiceResult<T> Create(ServiceError error) => new(error);

        /// <summary>
        /// Gets the current union value.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Gets a value indicating whether the union has a value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Attempts to get the successful value.
        /// </summary>
        public bool TryGetValue(out T value);

        /// <summary>
        /// Attempts to get the service error.
        /// </summary>
        public bool TryGetValue(out ServiceError value);
    }
}
