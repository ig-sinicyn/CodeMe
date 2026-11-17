using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults;

[Union]
public readonly struct ServiceResult<T> : IEquatable<ServiceResult<T>>, ServiceResult<T>.IUnionMembers
{
    public static bool operator ==(ServiceResult<T> left, ServiceResult<T> right) => Equals(left, right);

    public static bool operator !=(ServiceResult<T> left, ServiceResult<T> right) => !(left == right);

    public static implicit operator ServiceResult<T>(T value) => new(value);

    public static implicit operator ServiceResult<T>(ServiceError error) => new(error);

    public static implicit operator ServiceResult(ServiceResult<T> result) =>
        result is ServiceError error ? error : new OkResult();

    private readonly T _value;
    private readonly ServiceError? _error;

    public ServiceResult(T value)
    {
        _value = value;
        _error = null;
    }

    public ServiceResult(ServiceError error)
    {
        _value = default!;
        _error = error;
    }

    public bool IsError => _error != null;

    public T Value => _error == null
        ? _value
        : throw new ServiceException(_error.Value);

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

    public bool Equals(ServiceResult<T> other) =>
        _error?.Equals(other._error)
        ?? EqualityComparer<T>.Default.Equals(_value, other._value);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ServiceResult<T> x && Equals(x);

    public override int GetHashCode() =>
        _error?.GetHashCode()
        ?? EqualityComparer<T>.Default.GetHashCode(_value);

    public interface IUnionMembers
    {
        public static ServiceResult<T> Create(T value) => new(value);

        public static ServiceResult<T> Create(ServiceError error) => new(error);

        public object? Value { get; }

        public bool HasValue { get; }

        public bool TryGetValue(out T value);

        public bool TryGetValue(out ServiceError value);
    }
}