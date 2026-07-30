using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults;

public readonly record struct ServiceResult<T>(T Value, ServiceError? Error = null)
{
    public static implicit operator ServiceResult<T>(T value) => new(value);

    public static implicit operator ServiceResult<T>(ServiceError error) => new(error);

    public static implicit operator ServiceResult(ServiceResult<T> result) => new(result.Error);

    public static bool operator true(ServiceResult<T> serviceResult) => serviceResult.Error == null;

    public static bool operator false(ServiceResult<T> serviceResult) => serviceResult.Error != null;

    public ServiceResult(ServiceError error) : this(default!, error)
    {
    }

    public bool IsError => Error != null;
}