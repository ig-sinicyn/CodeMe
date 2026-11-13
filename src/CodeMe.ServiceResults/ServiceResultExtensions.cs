namespace CodeMe.ServiceResults;

public static class ServiceResultExtensions
{
    public static ServiceResult With(this ServiceResult source, Func<ServiceResult> callback) =>
        source.Error ?? callback();

    public static ServiceResult<T> With<T>(this ServiceResult source, Func<ServiceResult<T>> callback) =>
        source.Error ?? callback();

    public static ServiceResult With<T>(this ServiceResult<T> source, Func<T, ServiceResult> callback) =>
        source.Error ?? callback(source.Value);

    public static ServiceResult<T2> With<T1, T2>(this ServiceResult<T1> source, Func<T1, ServiceResult<T2>> callback) =>
        source.Error ?? callback(source.Value);

    public static ValueTask<ServiceResult> WithAsync(
        this ServiceResult source,
        Func<ValueTask<ServiceResult>> callback) =>
        source.Error is { } error
            ? new ValueTask<ServiceResult>(error)
            : callback();

    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ServiceResult source,
        Func<ValueTask<ServiceResult<T>>> callback) =>
        source.Error is { } error
            ? new ValueTask<ServiceResult<T>>(error)
            : callback();

    public static ValueTask<ServiceResult> WithAsync<T>(
        this ServiceResult<T> source,
        Func<T, ValueTask<ServiceResult>> callback) =>
        source.Error is { } error
            ? new ValueTask<ServiceResult>(error)
            : callback(source.Value);

    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ServiceResult<T1> source,
        Func<T1, ValueTask<ServiceResult<T2>>> callback) =>
        source.Error is { } error
            ? new ValueTask<ServiceResult<T2>>(error)
            : callback(source.Value);

    public static ValueTask<ServiceResult> WithAsync(
        this ValueTask<ServiceResult> source,
        Func<ServiceResult> callback)
    {
        return source.IsCompleted
            ? new ValueTask<ServiceResult>(source.Result.With(callback))
            : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ServiceResult> callback)
        {
            var result = await task;
            return result.With(callback);
        }
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
            Func<ServiceResult<T>> callback)
        {
            var result = await task;
            return result.With(callback);
        }
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
            Func<T, ServiceResult> callback)
        {
            var result = await task;
            return result.With(callback);
        }
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
            Func<T1, ServiceResult<T2>> callback)
        {
            var result = await task;
            return result.With(callback);
        }
    }

    public static ValueTask<ServiceResult> WithAsync(
        this ValueTask<ServiceResult> source,
        Func<ValueTask<ServiceResult>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ValueTask<ServiceResult>> callback)
        {
            var result = await task;
            return await result.WithAsync(callback);
        }
    }

    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ValueTask<ServiceResult> source,
        Func<ValueTask<ServiceResult<T>>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T>> CompleteAsync(
            ValueTask<ServiceResult> task,
            Func<ValueTask<ServiceResult<T>>> callback)
        {
            var result = await task;
            return await result.WithAsync(callback);
        }
    }

    public static ValueTask<ServiceResult> WithAsync<T>(
        this ValueTask<ServiceResult<T>> source,
        Func<T, ValueTask<ServiceResult>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult> CompleteAsync(
            ValueTask<ServiceResult<T>> task,
            Func<T, ValueTask<ServiceResult>> callback)
        {
            var result = await task;
            return await result.WithAsync(callback);
        }
    }

    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ValueTask<ServiceResult<T1>> source,
        Func<T1, ValueTask<ServiceResult<T2>>> callback)
    {
        return source.IsCompleted ? source.Result.WithAsync(callback) : CompleteAsync(source, callback);

        static async ValueTask<ServiceResult<T2>> CompleteAsync(
            ValueTask<ServiceResult<T1>> task,
            Func<T1, ValueTask<ServiceResult<T2>>> callback)
        {
            var result = await task;
            return await result.WithAsync(callback);
        }
    }
}