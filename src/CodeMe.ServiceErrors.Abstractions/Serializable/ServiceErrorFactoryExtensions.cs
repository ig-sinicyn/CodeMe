namespace CodeMe.ServiceErrors.Serializable;

/// <summary>
/// Extension methods for <see cref="IServiceErrorFactory"/> to create DTOs and exceptions in one step.
/// </summary>
public static class ServiceErrorFactoryExtensions
{
    /// <summary>
    /// Creates a serializable <see cref="ServiceErrorDto"/> from <see cref="Exception"/>.
    /// </summary>
    public static ServiceErrorDto CreateDto(this IServiceErrorFactory factory, Exception exception) =>
        factory.CreateDto(factory.CreateError(exception));

    /// <summary>
    /// Creates exception from <see cref="ServiceErrorDto"/>.
    /// </summary>
    public static IServiceException CreateException(this IServiceErrorFactory factory, ServiceErrorDto error) =>
        factory.CreateException(factory.CreateError(error));
}