
# ScopedAsyncLocal<T>

`ScopedAsyncLocal<T>` provides ambient context for a logical execution flow. It is useful when a value should be available to all code in the current flow without explicitly passing it through every method call. Typical scenarios include unit of work scopes, request correlation identifiers, tenant information, etc.

## Scenarios and example of usage

Use `ScopedAsyncLocal<T>` when a value should be available to the current logical execution path and reverted automatically when the scope ends. The value is visible to nested scopes and is restored to the previous ambient value when the scope is disposed.

```csharp
using CodeMe.Basics.Threading;

var context = new ScopedAsyncLocal<string>();

using (context.BeginScope("request-1"))
{
    Console.WriteLine(context.Current); // request-1

    using (context.BeginScope("nested"))
    {
        Console.WriteLine(context.Current); // nested
    }

    Console.WriteLine(context.Current); // request-1
}

Console.WriteLine(context.Current); // null
```
n t
`ScopedAsyncLocal<T>` also works with asynchronous flows. The value from the current scope is available after `await`; the scope must be disposed to restore the previous value.

```csharp
var local = new ScopedAsyncLocal<string>();

using (await local.BeginScopeAsync(() => new ValueTask<string>("request-2")))
{
    Console.WriteLine(local.Current); // request-2
}
```

The constructor can be used with `validateDisposeOrder: true` to detect out-of-order scope disposal.

## Using BeginScopeAsync from helper methods

`BeginScopeAsync` initializes the scope asynchronously through a value factory. If you call it from a helper method, keep the helper synchronous and avoid `await` in the call. Otherwise new scope will not be propagated back to the caller. The value factory itself may use `await`.

```csharp
    private Task<IDisposable> BeginUnitOfWorkAsync(
        ScopedAsyncLocal<IUnitOfWork> local,
        CancellationToken cancellation = default) =>
        local.BeginScopeAsync(
            async () =>
            {
                // The body is simplified for demonstration purposes.
                DbConnection? connection = null;
                DbTransaction? transaction = null;
                try
                {
                    connection = await _connectionFactory.CreateConnectionAsync(cancellation);
                    await connection.OpenAsync(cancellation);
                    transaction = await connection.BeginTransactionAsync(cancellation);

                    return new UnitOfWork(connection, transaction);
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        await transaction.DisposeAsync();
                    }

                    if (connection != null)
                    {
                        await connection.DisposeAsync();
                    }

                    throw;
                }
            });
```