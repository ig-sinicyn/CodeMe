using System.Diagnostics.CodeAnalysis;

namespace CodeMe.Data.UnitOfWork;

public interface IUnitOfWorkAccessorProvider<TUnitOfWorkAccessor>
    where TUnitOfWorkAccessor : IUnitOfWorkNamedAccessor
{
    TUnitOfWorkAccessor this[string dbName] => GetAccessor(dbName);

    TUnitOfWorkAccessor GetAccessor(string dbName);

    bool TryGetAccessor(
        string dbName,
        [MaybeNullWhen(false)] out TUnitOfWorkAccessor unitOfWorkAccessor);
}