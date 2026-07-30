using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults;

public readonly record struct ServiceResult(ServiceError? Error = null)
{
    public static implicit operator ServiceResult(ServiceError error) => new(error);

    public static ServiceResult Ok() => new();

    public static ServiceResult FromError(ServiceError error) => new(error);

    public static ServiceResult<T> Ok<T>(T value) => new(value);

    public static ServiceResult<T> FromError<T>(ServiceError error) => new(error);

    public bool IsError => Error != null;
}