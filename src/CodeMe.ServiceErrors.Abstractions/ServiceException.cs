namespace CodeMe.ServiceErrors;

/// <summary>
/// Base exception for <see cref="ServiceError"/>.
/// </summary>
public class ServiceException : Exception, IServiceException
{
    /// <summary>
    /// Checks if the <paramref name="error"/> matches the <paramref name="expected"/> descriptor.
    /// <see cref="ErrorDescriptor.Type"/> and <see cref="ErrorDescriptor.StatusCode"/> are checked.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <see cref="ErrorDescriptor.Type"/> and <see cref="ErrorDescriptor.StatusCode"/> do not match to expected values.
    /// </exception>
    protected static ServiceError AssertMatches(ErrorDescriptor expected, ServiceError error)
    {
        if (!error.Matches(expected.Type))
        {
            throw new ArgumentException(
                $"{error.Descriptor.Type} should be {expected.Type}",
                nameof(error));
        }

        if (!error.Matches(expected.StatusCode))
        {
            throw new ArgumentException(
                $"{error.Descriptor.StatusCode} should be {expected.StatusCode}",
                nameof(error));
        }

        return error;
    }

    /// <summary>
    /// Base exception for <see cref="ServiceError"/>.
    /// </summary>
    public ServiceException(ServiceError error) : base(error.Message, error.InnerException)
    {
        Error = error;
    }

    /// <summary>
    /// Base exception for <see cref="ServiceError"/>.
    /// </summary>
    public ServiceException(
        ErrorDescriptor descriptor,
        string message)
        : this(new ServiceError(descriptor, message))
    {
    }

    /// <summary>
    /// Base exception for <see cref="ServiceError"/>.
    /// </summary>
    public ServiceException(
        ErrorDescriptor descriptor,
        string message,
        Exception? innerException)
        : this(
            new ServiceError(descriptor, message)
            {
                InnerException = innerException
            })
    {
    }

    /// <inheritdoc/>
    public ServiceError Error { get; }
}