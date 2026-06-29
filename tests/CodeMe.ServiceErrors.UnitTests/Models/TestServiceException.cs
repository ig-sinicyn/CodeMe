namespace CodeMe.ServiceErrors.UnitTests.Models;

public class TestServiceException : ServiceException
{
    public TestServiceException(ServiceError error) : base(error)
    {
    }

    public TestServiceException(ErrorDescriptor descriptor, string message) : base(descriptor, message)
    {
    }

    public TestServiceException(ErrorDescriptor descriptor, string message, Exception? innerException) : base(descriptor, message, innerException)
    {
    }
}