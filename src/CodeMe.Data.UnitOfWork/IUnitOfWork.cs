using System.Data.Common;

namespace CodeMe.Data.UnitOfWork;

public interface IUnitOfWork
{
    DbConnection Connection { get; }

    DbTransaction? Transaction { get; }
}