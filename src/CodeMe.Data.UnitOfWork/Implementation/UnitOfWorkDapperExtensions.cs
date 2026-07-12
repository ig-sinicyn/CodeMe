using Dapper;

namespace CodeMe.Data.UnitOfWork.Implementation;

/// <summary>
/// <see cref="IUnitOfWork"/> Dapper extension methods.
/// </summary>
public static class UnitOfWorkDapperExtensions
{
    /// <summary>
    /// Execute a command asynchronously using Task.
    /// </summary>
    /// <param name="unitOfWork">The connection and transaction to execute on.</param>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="cancellation">The cancellation token for this command.</param>
    /// <returns>The number of rows affected.</returns>
    public static Task<int> ExecuteAsync(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Connection.ExecuteAsync(
            unitOfWork.ToCommandDefinition(commandText, parameters, cancellation));
    }

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="unitOfWork">The connection and transaction to execute on.</param>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="cancellation">The cancellation token for this command.</param>
    /// <returns>The number of rows affected.</returns>
    public static Task<T?> ExecuteScalarAsync<T>(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Connection.ExecuteScalarAsync<T>(
            unitOfWork.ToCommandDefinition(commandText, parameters, cancellation));
    }

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="unitOfWork">The connection and transaction to execute on.</param>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="cancellation">The cancellation token for this command.</param>
    /// <returns>The number of rows affected.</returns>
    public static Task<T?> QueryFirstOrDefaultAsync<T>(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Connection.QueryFirstOrDefaultAsync<T>(
            unitOfWork.ToCommandDefinition(commandText, parameters, cancellation));
    }

    /// <summary>
    /// Execute a single-row query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="unitOfWork">The connection and transaction to execute on.</param>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="cancellation">The cancellation token for this command.</param>
    /// <returns>The number of rows affected.</returns>
    public static Task<T> QueryFirstAsync<T>(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Connection.QueryFirstAsync<T>(
            unitOfWork.ToCommandDefinition(commandText, parameters, cancellation));
    }

    /// <summary>
    /// Execute a query asynchronously using Task.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="unitOfWork">The connection and transaction to execute on.</param>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="cancellation">The cancellation token for this command.</param>
    /// <returns>
    /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string etc.)
    /// is queried then the data from the first column is assumed, otherwise an instance is
    /// created per row, and a direct column-name===member-name mapping is assumed (case-insensitive).
    /// </returns>
    public static Task<IEnumerable<T>> QueryAsync<T>(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        return unitOfWork.Connection.QueryAsync<T>(
            unitOfWork.ToCommandDefinition(commandText, parameters, cancellation));
    }

    private static CommandDefinition ToCommandDefinition(
        this IUnitOfWork unitOfWork,
        string commandText,
        object? parameters = null,
        CancellationToken cancellation = default) =>
        new(
            commandText,
            parameters,
            unitOfWork.Transaction,
            cancellationToken: cancellation);
}