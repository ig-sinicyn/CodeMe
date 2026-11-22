using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CodeMe.ServiceErrors.Results;

[Union]
public readonly struct ServiceResult : IEquatable<ServiceResult>, ServiceResult.IUnionMembers
{
    public static bool operator ==(ServiceResult left, ServiceResult right) => Equals(left, right);

    public static bool operator !=(ServiceResult left, ServiceResult right) => !(left == right);

    public static implicit operator ServiceResult(ServiceError error) => new(error);

    public static implicit operator ServiceResult(OkResult ok) => new();

    public static ServiceResult Ok() => new();

    public static ServiceResult FromError(ServiceError error) => new(error);

    public static ServiceResult<T> Ok<T>(T value) => new(value);

    public static ServiceResult<T> FromError<T>(ServiceError error) => new(error);

    private readonly ServiceError? _error;

    public ServiceResult()
    {
        _error = null;
    }

    public ServiceResult(ServiceError error)
    {
        _error = error;
    }

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => Error != null;

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

    public bool Equals(ServiceResult other) =>
        _error switch
        {
            { } error when other._error is { } otherError => error.Equals(otherError),
            null when other._error is null => true,
            _ => false
        };

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ServiceResult result && Equals(result);

    public override int GetHashCode() => _error?.GetHashCode() ?? 0;

    public interface IUnionMembers
    {
        public static ServiceResult Create(OkResult ok) => new();

        public static ServiceResult Create(ServiceError error) => new(error);

        public object Value { get; }

        public bool HasValue { get; }

        public bool TryGetValue(out OkResult value);

        public bool TryGetValue(out ServiceError value);
    }
}