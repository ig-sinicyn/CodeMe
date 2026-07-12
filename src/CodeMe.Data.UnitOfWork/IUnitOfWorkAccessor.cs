using System.Data;

namespace CodeMe.Data.UnitOfWork;

public interface IUnitOfWorkAccessor
{
    IUnitOfWork? Current { get; }

    Task<IScopedUnitOfWork> BeginAsync(
        UnitOfWorkOption unitOfWorkOption,
        IsolationLevel isolationLevel,
        CancellationToken cancellation = default);
}