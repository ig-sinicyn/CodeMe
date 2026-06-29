namespace CodeMe.ServiceErrors;

/// <summary>
/// Exception contract for <see cref="ServiceError"/>.
/// </summary>
public interface IServiceException
{
    /// <summary>
    /// Service error details.
    /// </summary>
    ServiceError Error { get; }
}