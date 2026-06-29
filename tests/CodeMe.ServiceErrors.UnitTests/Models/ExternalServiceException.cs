namespace CodeMe.ServiceErrors.UnitTests.Models;

public class ExternalServiceException : ServiceException
{
    public ExternalServiceException(ServiceError error) : base(error)
    {
    }

    public ExternalServiceException(ErrorDescriptor descriptor, string message) : base(descriptor, message)
    {
    }

    public ExternalServiceException(ErrorDescriptor descriptor, string message, Exception? innerException) : base(
        descriptor, message, innerException)
    {
    }
}