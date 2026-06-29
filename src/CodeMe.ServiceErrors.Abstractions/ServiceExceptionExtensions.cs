namespace CodeMe.ServiceErrors;

/// <summary>
/// Extensions for <see cref="IServiceException"/>
/// </summary>
public static class ServiceExceptionExtensions
{
    extension(IServiceException exception)
    {
        /// <summary>
        /// Error descriptor.
        /// </summary>
        public ErrorDescriptor Descriptor => exception.Error.Descriptor;

        /// <summary>
        /// Debug-only inner errors (obtained from <see cref="Exception.InnerException"/>).
        /// </summary>
        public IReadOnlyCollection<ServiceError> InnerErrors => exception.Error.InnerErrors;
    }
}