namespace CodeMe.ServiceErrors.Results;

public static class ChainingExtensions
{
    public static ServiceResult With(this ServiceResult source, Func<ServiceResult> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => error
        };

    public static ServiceResult<T> With<T>(this ServiceResult source, Func<ServiceResult<T>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => error
        };

    public static ServiceResult With<T>(this ServiceResult<T> source, Func<T, ServiceResult> callback) =>
        source switch
        {
            T value => callback(value),
            null => callback(default!),
            ServiceError error => error
        };

    public static ServiceResult<T2> With<T1, T2>(this ServiceResult<T1> source, Func<T1, ServiceResult<T2>> callback) =>
        source switch
        {
            T1 value => callback(value),
            null => callback(default!),
            ServiceError error => error
        };

    public static ValueTask<ServiceResult> WithAsync(
        this ServiceResult source,
        Func<ValueTask<ServiceResult>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => new ValueTask<ServiceResult>(error)
        };

    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ServiceResult source,
        Func<ValueTask<ServiceResult<T>>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => new ValueTask<ServiceResult<T>>(error)
        };

    public static ValueTask<ServiceResult> WithAsync<T>(
        this ServiceResult<T> source,
        Func<T, ValueTask<ServiceResult>> callback) =>
        source switch
        {
            T value => callback(value),
            null => callback(default!),
            ServiceError error => new ValueTask<ServiceResult>(error)
        };

    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ServiceResult<T1> source,
        Func<T1, ValueTask<ServiceResult<T2>>> callback) =>
        source switch
        {
            T1 value => callback(value),
            null => callback(default!),
            ServiceError error => new ValueTask<ServiceResult<T2>>(error)
        };

    public static ValueTask<ServiceResult> WithAsync(
        this ValueTask<ServiceResult> source,
        Func<ServiceResult> callback)
    {
        return source.IsCompleted
            ? new ValueTask<ServiceResult>(source.Result.With(callback))
            : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ServiceResult> callback) => 
            (await task).With(callback);
    }

    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ValueTask<ServiceResult> source,
        Func<ServiceResult<T>> callback)
    {
        return source.IsCompleted
            ? new ValueTask<ServiceResult<T>>(source.Result.With(callback))
            : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T>> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ServiceResult<T>> callback) =>
            (await task).With(callback);
    }

    public static ValueTask<ServiceResult> WithAsync<T>(
        this ValueTask<ServiceResult<T>> source,
        Func<T, ServiceResult> callback)
    {
        return source.IsCompleted
            ? new ValueTask<ServiceResult>(source.Result.With(callback))
            : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult<T>> task,
            Func<T, ServiceResult> callback) =>
            (await task).With(callback);
    }

    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ValueTask<ServiceResult<T1>> source,
        Func<T1, ServiceResult<T2>> callback)
    {
        return source.IsCompleted
            ? new ValueTask<ServiceResult<T2>>(source.Result.With(callback))
            : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T2>> CompleteAsync(
            ValueTask<ServiceResult<T1>> task,
            Func<T1, ServiceResult<T2>> callback) =>
            (await task).With(callback);
    }

    public static ValueTask<ServiceResult> WithAsync(
        this ValueTask<ServiceResult> source,
        Func<ValueTask<ServiceResult>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ValueTask<ServiceResult>> callback) =>
            await (await task).WithAsync(callback);
    }

    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ValueTask<ServiceResult> source,
        Func<ValueTask<ServiceResult<T>>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T>> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ValueTask<ServiceResult<T>>> callback) =>
            await (await task).WithAsync(callback);
    }

    public static ValueTask<ServiceResult> WithAsync<T>(
        this ValueTask<ServiceResult<T>> source,
        Func<T, ValueTask<ServiceResult>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult<T>> task,
            Func<T, ValueTask<ServiceResult>> callback) =>
            await (await task).WithAsync(callback);
    }

    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ValueTask<ServiceResult<T1>> source,
        Func<T1, ValueTask<ServiceResult<T2>>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T2>> CompleteAsync(
            ValueTask<ServiceResult<T1>> task,
            Func<T1, ValueTask<ServiceResult<T2>>> callback) =>
            await (await task).WithAsync(callback);
    }
}