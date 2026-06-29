namespace CodeMe.ServiceErrors.Serializable;

/// <summary>
/// Methods for converting <see cref="ServiceErrorDto"/>, <see cref="ServiceError"/>, <see cref="IServiceException"/>.
/// </summary>
public interface IServiceErrorFactory
{
    /// <summary>
    /// Error group for unknown error codes.
    /// </summary>
    ErrorGroupUri RootErrorGroup { get; }

    /// <summary>
    /// Creates a serializable <see cref="ServiceErrorDto"/>.
    /// </summary>
    ServiceErrorDto CreateDto(ServiceError error);

    /// <summary>
    /// Creates error details from <see cref="ServiceErrorDto"/>.
    /// </summary>
    ServiceError CreateError(ServiceErrorDto error);

    /// <summary>
    /// Creates error details from <see cref="Exception"/>.
    /// </summary>
    ServiceError CreateError(Exception exception);

    /// <summary>
    /// Creates exception from <see cref="ServiceError"/>.
    /// </summary>
    IServiceException CreateException(ServiceError error);
}