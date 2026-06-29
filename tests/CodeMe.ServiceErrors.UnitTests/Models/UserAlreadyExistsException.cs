namespace CodeMe.ServiceErrors.UnitTests.Models;

public class UserAlreadyExistsException : Exception, IServiceException
{
    public UserAlreadyExistsException(ServiceError error)
    {
        Error = error;
    }

    public UserAlreadyExistsException(string message) : base(message)
    {
        Error = new ServiceError(WellKnownTestErrors.UserAlreadyExists, message);
    }

    public UserAlreadyExistsException(string message, Exception inner) : base(message, inner)
    {
        Error = new ServiceError(WellKnownTestErrors.UserAlreadyExists, message);
    }

    public ServiceError Error { get; }
}