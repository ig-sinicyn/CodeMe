
# ScopedAsyncLocal<T>

`ScopedAsyncLocal<T>` provides ambient context for a logical execution flow. It is useful when a value should be available to all code in the current flow without explicitly passing it through every method call. Typical scenarios include unit of work scopes, request correlation identifiers, tenant information, etc.

## How it works

`ScopedAsyncLocal<T>` provides ambient context by keeping a stack of scopes for the current execution flow. Each scope represents a logical boundary such as a request, unit-of-work, or tenant context. When you call `BeginScope` or `BeginScopeAsync`, a new scope is pushed onto the current execution context, and `Current` resolves to the value from the innermost initialized scope.

Nested scopes override the parent value while they are active. When the child scope is disposed, the previous ambient value is restored automatically, so the value behaves like a flow-scoped context without having to pass it through every method call.

The implementation uses `AsyncLocal<T>` under the hood, which means the value is captured per async flow. Because async calls create copies of the execution context, initialization is split into two steps:

- the scope is created synchronously so it can be observed immediately by the caller;
- the scope value is initialized later, and uninitialized scopes are ignored while resolving `Current`.

This design lets the new scope be available to the caller right away, while still supporting asynchronous value factories. The library also keeps a stack of scopes so it can restore the previous value when a scope is disposed. If you forget to dispose scopes, the stack can grow and hold references longer than intended. When `validateDisposeOrder: true` is used, disposing scopes in the wrong order throws an exception.

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