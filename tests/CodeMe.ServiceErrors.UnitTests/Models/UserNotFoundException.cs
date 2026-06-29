namespace CodeMe.ServiceErrors.UnitTests.Models;

public class UserNotFoundException : TestServiceException
{
    public UserNotFoundException(ServiceError error)
        : base(AssertMatches(WellKnownTestErrors.UserNotFound, error))
    {
    }

    public UserNotFoundException(string message)
        : base(WellKnownTestErrors.UserNotFound, message)
    {
    }

    public UserNotFoundException(string message, Exception inner)
        : base(WellKnownTestErrors.UserNotFound, message, inner)
    {
    }
}