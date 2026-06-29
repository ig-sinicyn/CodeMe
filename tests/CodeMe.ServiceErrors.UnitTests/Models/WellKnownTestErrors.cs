using CodeMe.ServiceErrors.DependencyInjection;

namespace CodeMe.ServiceErrors.UnitTests.Models;

[ServiceErrors]
internal static class WellKnownTestErrors
{
    [ServiceException(typeof(TestServiceException))]
    public static readonly ErrorGroupUri RootGroup = ErrorGroupUri.Create("problem", "test-service");

    public static readonly ErrorGroupUri UnknownErrorsGroup = RootGroup.SubGroup("unknowns");

    public static readonly ErrorGroupUri UsersGroup = RootGroup.SubGroup("users");

    public static readonly ErrorGroupUri InfrastructureGroup = RootGroup.SubGroup("infrastructure");

    [ServiceException<UserNotFoundException>]
    public static readonly ErrorDescriptor UserNotFound = ErrorDescriptor.NotFound(UsersGroup, "user-not-found");

    [ServiceException<UserAlreadyExistsException>]
    public static readonly ErrorDescriptor UserAlreadyExists = ErrorDescriptor.AlreadyExists(UsersGroup, "user-already-exists");

    public static readonly ErrorDescriptor UnexpectedError = ErrorDescriptor.Internal(InfrastructureGroup, "unexpected-error");
}