namespace CodeMe.Data.UnitOfWork;

public enum UnitOfWorkOption
{
    /// <summary>
    /// Same as <see cref="System.Transactions.TransactionScopeOption.Required"/>.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Same as <see cref="System.Transactions.TransactionScopeOption.RequiresNew"/>.
    /// </summary>
    NewScope = 1,

    /// <summary>
    /// Same as <see cref="System.Transactions.TransactionScopeOption.Suppress"/>.
    /// </summary>
    NewScopeWithoutTransaction = 2
}