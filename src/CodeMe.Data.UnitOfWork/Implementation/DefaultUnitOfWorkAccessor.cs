using System.Data;
using System.Data.Common;
using CodeMe.Basics.Threading;

namespace CodeMe.Data.UnitOfWork.Implementation;

internal class DefaultUnitOfWorkAccessor : IUnitOfWorkNamedAccessor
{
    private static void AssertUnitOfWorkOption(UnitOfWorkOption value)
    {
        switch (value)
        {
            case UnitOfWorkOption.Auto:
            case UnitOfWorkOption.NewScope:
            case UnitOfWorkOption.NewScopeWithoutTransaction:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }

    private static void AssertNewScopeIsolationLevel(DbTransaction transaction, IsolationLevel isolationLevel)
    {
        var currentIsolationLevel = transaction.IsolationLevel;

        // BASED ON: TransactionScope validation logic
        // https://github.com/dotnet/runtime/blob/release/8.0/src/libraries/System.Transactions.Local/src/System/Transactions/TransactionScope.cs#L212-L216
        if (isolationLevel != IsolationLevel.Unspecified && transaction.IsolationLevel != isolationLevel)
        {
            throw new InvalidOperationException(
                $"Parent scope isolation level {currentIsolationLevel} does not match to the requester one ({isolationLevel}). "
                + $"Please use proper isolation level or explicit {nameof(UnitOfWorkOption)} mode.");
        }
    }

    private readonly Func<DbConnection> _connectionFactory;

    private readonly ScopedAsyncLocal<IUnitOfWork> _scopeAccessor;

    public DefaultUnitOfWorkAccessor(string? name, Func<DbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _scopeAccessor = new ScopedAsyncLocal<IUnitOfWork>();
        Name = name ?? "";
    }

    public string Name { get; }

    public IUnitOfWork? Current => _scopeAccessor.Current;

    public Task<IScopedUnitOfWork> BeginAsync(
        UnitOfWorkOption unitOfWorkOption,
        IsolationLevel isolationLevel,
        CancellationToken cancellation = default)
    {
        // Save scope in the current initialization context
        var scope = _scopeAccessor.BeginScopeInitialization();

        // Complete initialization (asynchronous part)
        return CompleteBeginAsync(scope, unitOfWorkOption, isolationLevel, cancellation);
    }

    private async Task<IScopedUnitOfWork> CompleteBeginAsync(
        ScopedAsyncLocal<IUnitOfWork>.Scope scopeToInitialise,
        UnitOfWorkOption unitOfWorkOption,
        IsolationLevel isolationLevel,
        CancellationToken cancellation = default)
    {
        AssertUnitOfWorkOption(unitOfWorkOption);

        DbConnection? connection = null;
        DbTransaction? transaction = null;
        var ownsConnection = false;
        var ownsTransaction = false;
        try
        {
            if (unitOfWorkOption == UnitOfWorkOption.Auto)
            {
                var currentUnitOfWork = _scopeAccessor.Current;
                if (currentUnitOfWork != null)
                {
                    connection = currentUnitOfWork.Connection;
                    transaction = currentUnitOfWork.Transaction;
                    if (transaction != null)
                    {
                        AssertNewScopeIsolationLevel(transaction, isolationLevel);
                    }
                }
            }

            if (connection == null)
            {
                connection = _connectionFactory();
                ownsConnection = true;
            }

            if (transaction == null && unitOfWorkOption != UnitOfWorkOption.NewScopeWithoutTransaction)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync(cancellation);
                }

                transaction = await connection.BeginTransactionAsync(isolationLevel, cancellation);
                ownsTransaction = true;
            }

            var unitOfWork = new ScopedUnitOfWork(
                scopeToInitialise,
                connection,
                transaction,
                ownsConnection,
                ownsTransaction);
            scopeToInitialise.Initialize(unitOfWork);

            return unitOfWork;
        }
        catch (Exception)
        {
            scopeToInitialise.Dispose();

            if (ownsTransaction && transaction != null)
            {
                await transaction.DisposeAsync();
            }

            if (ownsConnection && connection != null)
            {
                await connection.DisposeAsync();
            }

            throw;
        }
    }
}