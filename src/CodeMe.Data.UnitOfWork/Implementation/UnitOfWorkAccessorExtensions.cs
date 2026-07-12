using System.Data;

namespace CodeMe.Data.UnitOfWork.Implementation;

public static class UnitOfWorkAccessorExtensions
{
    // TODO: helpers to begin exclusive scope (fail if there is any).

    public static IUnitOfWork GetRequired(this IUnitOfWorkAccessor unitOfWorkAccessor)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkAccessor);
        return unitOfWorkAccessor.Current.AssertExists();
    }

    public static IUnitOfWork GetRequiredWithTransaction(this IUnitOfWorkAccessor unitOfWorkAccessor)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkAccessor);
        return unitOfWorkAccessor.Current
            .AssertExists()
            .AssertHasTransaction();
    }

    public static Task<IScopedUnitOfWork> BeginNewAsync(
        this IUnitOfWorkAccessor unitOfWorkAccessor,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkAccessor);
        return unitOfWorkAccessor.BeginAsync(
            UnitOfWorkOption.NewScope,
            isolationLevel,
            cancellation);
    }

    public static Task<IScopedUnitOfWork> BeginAutoAsync(
        this IUnitOfWorkAccessor unitOfWorkAccessor,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkAccessor);
        return unitOfWorkAccessor.BeginAsync(
            UnitOfWorkOption.Auto,
            isolationLevel,
            cancellation);
    }

    public static Task<IScopedUnitOfWork> BeginNewWithoutTransactionAsync(
        this IUnitOfWorkAccessor unitOfWorkAccessor,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkAccessor);
        return unitOfWorkAccessor.BeginAsync(
            UnitOfWorkOption.NewScopeWithoutTransaction,
            IsolationLevel.Unspecified,
            cancellation);
    }

    private static IUnitOfWork AssertExists(this IUnitOfWork? unitOfWork) =>
        unitOfWork ?? throw new InvalidOperationException("There is no active unit of work");

    private static IUnitOfWork AssertHasTransaction(this IUnitOfWork unitOfWork) =>
        unitOfWork.Transaction != null
            ? unitOfWork
            : throw new InvalidOperationException("Current unit of work has no transaction");
}