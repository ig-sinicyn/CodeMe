# Custom ambient contexts

There are many scenarios in which you do not want to expose `AsyncLocal` or `ScopedAsyncLocal` directly to external code. This document uses a simplified unit-of-work example to show how you can introduce a custom ambient context on top of `ScopedAsyncLocal<T>`.

Usage

```csharp
using System.Data;
using System.Data.Common;
using CodeMe.Threading;

var unitOfWorkManager = new UnitOfWorkManager();
var repository = new UserRepository(unitOfWorkManager); // Usually, this is injected via a DI container.

await using (var unitOfWork = await unitOfWorkManager.BeginUnitOfWorkAsync())
{
    var user = await repository.GetUserAsync(id: 123);

    // ...

    await unitOfWork.CommitAsync();
}

// UserRepository.GetUserAsync() is called within the context of an ambient unit of work.
async ValueTask<User> GetUserAsync(long id)
{
    var unitOfWork = unitOfWorkManager.RequiredCurrent;

    return await unitOfWork.Connection.QueryFirstAsync(...);
}
```

Implementation

```csharp
// Simplified version for demonstration purposes. In real-world scenarios, consider using a more robust implementation.
public class UnitOfWorkManager
{
    private readonly Func<ValueTask<DbConnection>> _connectionFactory;

    private readonly ScopedAsyncLocal<UnitOfWork> _scopedAsyncLocal =
        new ScopedAsyncLocal<UnitOfWork>(validateDisposeOrder: true);

    public UnitOfWork RequiredCurrent => _scopedAsyncLocal.Current
        ?? throw new InvalidOperationException("No ambient unit of work.");

    public ValueTask<UnitOfWork> BeginUnitOfWorkAsync(IsolationLevel isolation = IsolationLevel.ReadCommitted)
    {
        // IMPORTANT:
        // The BeginScopeInitialization method SHOULD be called in the synchronous part of the method.
        // Otherwise, the new AsyncLocal value will not be stored in the caller's execution context.
        var scope = _scopedAsyncLocal.BeginScopeInitialization();
        return BeginUnitOfWorkAsync(scope, isolation);
    }

    private async ValueTask<UnitOfWork> BeginUnitOfWorkAsync(
        ScopedAsyncLocal<UnitOfWork>.Scope scope,
        IsolationLevel isolation)
    {
        // Asynchronous part.
        // Performs a step-by-step initialization of the UnitOfWork instance
        // and assigns it to the scope. If any exception occurs, the UnitOfWork instance (and all related resources) will be disposed.
        var result = new UnitOfWork();
        try
        {
            result.Initialize(scope);
            var connection = await _connectionFactory();
            result.Initialize(connection);
            await connection.OpenAsync();

            var transaction = await connection.BeginTransactionAsync(isolation);
            result.Initialize(transaction);

            // Assign the fully constructed UnitOfWork to the scope.
            // This is the only place where the scope value is set.
            scope.Initialize(result);

            return result;
        }
        catch (Exception)
        {
            await result.DisposeAsync();
            throw;
        }
    }
}

public sealed class UnitOfWork : IAsyncDisposable
{
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private IDisposable _asyncScope;
    private bool _transactionClosed;

    public DbConnection Connection { get; private set; }

    public DbTransaction Transaction { get; private set; }

    internal void Initialize(DbConnection connection) => _connection = connection;

    internal void Initialize(DbTransaction transaction) => _transaction = transaction;

    internal void Initialize(IDisposable asyncScope) => _asyncScope = asyncScope;

    public async ValueTask CommitAsync()
    {
        await _transaction.CommitAsync();
        _transactionClosed = true;
    }

    public async ValueTask RollbackAsync()
    {
        await _transaction.CommitAsync();
        _transactionClosed = true;
    }

    public ValueTask DisposeAsync()
    {
        // IMPORTANT:
        // For performance-sensitive code, it is recommended to dispose the scope in the synchronous part of DisposeAsync.
        // This slightly reduces memory usage, because it clears the AsyncLocal value in the caller's execution context.
        _asyncScope?.Dispose();
        return DisposeCoreAsync();
    }

    private async ValueTask DisposeCoreAsync()
    {
        // Asynchronous part. Nothing special here.
        if (!_transactionClosed)
        {
            await _transaction.RollbackAsync();
        }

        await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
```
