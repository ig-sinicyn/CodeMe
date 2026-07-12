using System.Data.Common;
using CodeMe.Basics.Threading;

namespace CodeMe.Data.UnitOfWork.Implementation;

internal sealed class ScopedUnitOfWork : IScopedUnitOfWork
{
    private readonly ScopedAsyncLocal<IUnitOfWork>.Scope _ownedScope;
    private readonly bool _ownsConnection;
    private readonly bool _ownsTransaction;

    public ScopedUnitOfWork(
        ScopedAsyncLocal<IUnitOfWork>.Scope ownedScope,
        DbConnection connection,
        DbTransaction? transaction,
        bool ownsConnection,
        bool ownsTransaction)
    {
        _ownedScope = ownedScope;
        _ownsConnection = ownsConnection;
        _ownsTransaction = transaction != null && (ownsConnection || ownsTransaction);

        Connection = connection;
        Transaction = transaction;
    }

    public DbConnection Connection { get; }

    public DbTransaction? Transaction { get; }

    public async ValueTask CommitAsync(CancellationToken cancellation = default)
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Current unit of work has no transaction");
        }

        // TODO: savepoint support for nested transactions
        if (_ownsTransaction)
        {
            await Transaction.CommitAsync(cancellation);
        }
    }

    public async ValueTask RollbackAsync(CancellationToken cancellation = default)
    {
        if (Transaction == null)
        {
            throw new InvalidOperationException("Current unit of work has no transaction");
        }

        // TODO: savepoint support for nested transactions
        await Transaction.RollbackAsync(cancellation);
    }

    public ValueTask DisposeAsync()
    {
        // Clean current execution context (executed synchronously).CompleteDisposeAsync
        _ownedScope.Dispose();

        // Complete dispose (asynchronous part)
        return CompleteDisposeAsync();
    }

    private async ValueTask CompleteDisposeAsync()
    {
        if (Transaction != null && _ownsTransaction)
        {
            await Transaction.DisposeAsync();
        }

        if (_ownsConnection)
        {
            await Connection.DisposeAsync();
        }
    }
}