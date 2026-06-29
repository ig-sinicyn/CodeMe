namespace CodeMe.ServiceErrors.UnitTests.Models;

internal static class WellKnownExternalTestErrors
{
    [ServiceException(typeof(ExternalServiceException))]
    public static readonly ErrorGroupUri ExternalRootGroup = ErrorGroupUri.Create("external", "other-service");

    public static readonly ErrorGroupUri ExternalPaymentsGroup = ExternalRootGroup.SubGroup("payments");

    public static readonly ErrorDescriptor ExternalInvalidAmountError =
        ErrorDescriptor.InvalidArgument(ExternalPaymentsGroup, "invalid-amount");
}