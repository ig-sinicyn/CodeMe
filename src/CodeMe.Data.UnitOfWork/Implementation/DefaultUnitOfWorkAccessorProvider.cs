namespace CodeMe.Data.UnitOfWork.Implementation;

internal class DefaultUnitOfWorkAccessorProvider<TUnitOfWorkAccessor>
    : IUnitOfWorkAccessorProvider<TUnitOfWorkAccessor>
    where TUnitOfWorkAccessor : IUnitOfWorkNamedAccessor
{
    private readonly Dictionary<string, TUnitOfWorkAccessor> _accessorsDictionary;

    public DefaultUnitOfWorkAccessorProvider(IEnumerable<TUnitOfWorkAccessor> unitOfWorkAccessors)
    {
        _accessorsDictionary =
            unitOfWorkAccessors.ToDictionary(
                key => key.Name,
                value => value,
                StringComparer.OrdinalIgnoreCase)
            ?? throw new ArgumentNullException(nameof(unitOfWorkAccessors));
    }

    public TUnitOfWorkAccessor GetAccessor(string dbName) =>
        _accessorsDictionary.TryGetValue(dbName, out var unitOfWorkAccessor)
            ? unitOfWorkAccessor
            : throw new KeyNotFoundException(dbName);

    public bool TryGetAccessor(string dbName, out TUnitOfWorkAccessor unitOfWorkAccessor) =>
        _accessorsDictionary.TryGetValue(dbName, out unitOfWorkAccessor!);
}