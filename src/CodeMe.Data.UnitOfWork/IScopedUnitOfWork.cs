namespace CodeMe.Data.UnitOfWork;

public interface IScopedUnitOfWork : IUnitOfWork, IAsyncDisposable
{
    ValueTask CommitAsync(CancellationToken cancellation = default);

    ValueTask RollbackAsync(CancellationToken cancellation = default);
}