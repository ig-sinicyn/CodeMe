namespace CodeMe.ServiceErrors.Results;

/// <summary>
/// Provides extension methods for chaining operations on service results.
/// </summary>
public static class ChainingExtensions
{
    /// <summary>
    /// Executes a callback when the source result is successful.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ServiceResult With(this ServiceResult source, Func<ServiceResult> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => error
        };

    /// <summary>
    /// Executes a callback and returns a typed result when the source result is successful.
    /// </summary>
    /// <typeparam name="T">The callback result value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ServiceResult<T> With<T>(this ServiceResult source, Func<ServiceResult<T>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => error
        };

    /// <summary>
    /// Executes a callback with the source value when the typed result is successful.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ServiceResult With<T>(this ServiceResult<T> source, Func<T, ServiceResult> callback) =>
        source switch
        {
            T value => callback(value),
            null => callback(default!),
            ServiceError error => error
        };

    /// <summary>
    /// Executes a callback with the source value and returns a second typed result when successful.
    /// </summary>
    /// <typeparam name="T1">The source value type.</typeparam>
    /// <typeparam name="T2">The callback result value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ServiceResult<T2> With<T1, T2>(this ServiceResult<T1> source, Func<T1, ServiceResult<T2>> callback) =>
        source switch
        {
            T1 value => callback(value),
            null => callback(default!),
            ServiceError error => error
        };

    /// <summary>
    /// Asynchronously executes a callback when the source result is successful.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The asynchronous operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ValueTask<ServiceResult> WithAsync(
        this ServiceResult source,
        Func<ValueTask<ServiceResult>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => new ValueTask<ServiceResult>(error)
        };

    /// <summary>
    /// Asynchronously executes a callback and returns a typed result when the source result is successful.
    /// </summary>
    /// <typeparam name="T">The callback result value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The asynchronous operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ValueTask<ServiceResult<T>> WithAsync<T>(
        this ServiceResult source,
        Func<ValueTask<ServiceResult<T>>> callback) =>
        source switch
        {
            OkResult => callback(),
            ServiceError error => new ValueTask<ServiceResult<T>>(error)
        };

    /// <summary>
    /// Asynchronously executes a callback with the source value when the typed result is successful.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The asynchronous operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ValueTask<ServiceResult> WithAsync<T>(
        this ServiceResult<T> source,
        Func<T, ValueTask<ServiceResult>> callback) =>
        source switch
        {
            T value => callback(value),
            null => callback(default!),
            ServiceError error => new ValueTask<ServiceResult>(error)
        };

    /// <summary>
    /// Asynchronously executes a callback with the source value and returns a second typed result when successful.
    /// </summary>
    /// <typeparam name="T1">The source value type.</typeparam>
    /// <typeparam name="T2">The callback result value type.</typeparam>
    /// <param name="source">The source result.</param>
    /// <param name="callback">The asynchronous operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
    public static ValueTask<ServiceResult<T2>> WithAsync<T1, T2>(
        this ServiceResult<T1> source,
        Func<T1, ValueTask<ServiceResult<T2>>> callback) =>
        source switch
        {
            T1 value => callback(value),
            null => callback(default!),
            ServiceError error => new ValueTask<ServiceResult<T2>>(error)
        };

    /// <summary>
    /// Awaits a source result and executes a synchronous callback on success.
    /// </summary>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a source result and executes a synchronous callback that returns a typed result on success.
    /// </summary>
    /// <typeparam name="T">The callback result value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a typed source result and executes a synchronous callback with its value on success.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a typed source result and executes a synchronous callback that returns a second typed result on success.
    /// </summary>
    /// <typeparam name="T1">The source value type.</typeparam>
    /// <typeparam name="T2">The callback result value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a source result and executes an asynchronous callback on success.
    /// </summary>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The asynchronous operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a source result and executes an asynchronous callback that returns a typed result on success.
    /// </summary>
    /// <typeparam name="T">The callback result value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The asynchronous operation to execute on success.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a typed source result and executes an asynchronous callback with its value on success.
    /// </summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The asynchronous operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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

    /// <summary>
    /// Awaits a typed source result and executes an asynchronous callback that returns a second typed result on success.
    /// </summary>
    /// <typeparam name="T1">The source value type.</typeparam>
    /// <typeparam name="T2">The callback result value type.</typeparam>
    /// <param name="source">The asynchronous source result.</param>
    /// <param name="callback">The asynchronous operation to execute with the successful value.</param>
    /// <returns>The callback result on success or the source error on failure.</returns>
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