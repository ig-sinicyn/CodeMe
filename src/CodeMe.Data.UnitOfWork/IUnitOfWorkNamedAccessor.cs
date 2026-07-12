namespace CodeMe.Data.UnitOfWork;

public interface IUnitOfWorkNamedAccessor : IUnitOfWorkAccessor
{
    string Name { get; }
}