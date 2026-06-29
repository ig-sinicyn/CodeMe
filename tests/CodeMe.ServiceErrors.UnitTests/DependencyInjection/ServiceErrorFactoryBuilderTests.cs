using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.UnitTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeMe.ServiceErrors.UnitTests.DependencyInjection;

public class ServiceErrorFactoryBuilderTests
{
    [Fact]
    public void DefaultFactory_ShouldBeExpected()
    {
        // Arrange
        using var services = new ServiceCollection()
            .AddServiceErrors(WellKnownTestErrors.RootGroup)
            .Build()
            .BuildServiceProvider();
        var options = services.GetRequiredService<IOptions<ServiceErrorFactoryOptions<IServiceErrorFactory>>>();
        var factory = services.GetRequiredService<IServiceErrorFactory>();

        // Act
        var error = factory.CreateError(
            new ServiceErrorDto
            {
                Code = "test-error",
                Message = "This is a test error."
            });

        // Assert
        options.Value.Should().BeEquivalentTo(
            DefaultServiceErrorFactoryOptions.Create(WellKnownTestErrors.RootGroup) with
            {
                CategoryFillMode = ErrorDtoFillMode.Always
            });
        var exception = factory.CreateException(error);

        error.Descriptor.Should().Be(
            ErrorDescriptor.Internal(WellKnownTestErrors.RootGroup, "test-error"));
        error.Message.Should().Be("This is a test error.");
        exception.Should().BeOfType<ServiceException>();
        exception.Error.Should().Be(error);
    }

    [Fact]
    public void WellKnownErrors_ShouldBeExpected()
    {
        // Arrange
        using var services = new ServiceCollection()
            .AddServiceErrors(WellKnownTestErrors.RootGroup)
            .Add(typeof(WellKnownTestErrors))
            .Build()
            .BuildServiceProvider();
        var factory = services.GetRequiredService<IServiceErrorFactory>();

        // Act
        var error = factory.CreateError(
            new ServiceErrorDto
            {
                Code = WellKnownTestErrors.UserNotFound.ShortCode,
                Message = "This is a test error."
            });
        var unexpectedError = factory.CreateError(
            new ServiceErrorDto
            {
                Code = "unknown-error",
                Message = "This is a test error."
            });
        var externalError = factory.CreateError(
            new ServiceErrorDto
            {
                Scheme = "external",
                Application = "other-service",
                Code = WellKnownExternalTestErrors.ExternalInvalidAmountError.ShortCode,
                Message = "This is a test error."
            });
        var exception = factory.CreateException(error);
        var unexpectedException = factory.CreateException(unexpectedError);
        var externalException = factory.CreateException(externalError);

        // Assert
        error.Should().Be(
            new ServiceError(WellKnownTestErrors.UserNotFound, "This is a test error."));
        unexpectedError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Internal(WellKnownTestErrors.RootGroup, "unknown-error"),
                "This is a test error."));
        externalError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Internal(WellKnownExternalTestErrors.ExternalRootGroup, "invalid-amount"),
                "This is a test error."));

        exception.Should().BeOfType<UserNotFoundException>();
        exception.Error.Should().Be(error);
        unexpectedException.Should().BeOfType<TestServiceException>();
        unexpectedException.Error.Should().Be(unexpectedError);
        externalException.Should().BeOfType<ServiceException>();
        externalException.Error.Should().Be(externalError);
    }

    [Fact]
    public void WellKnownErrorsInAssembly_ShouldBeExpected()
    {
        // Arrange
        using var services = new ServiceCollection()
            .AddServiceErrors(WellKnownTestErrors.RootGroup)
            .AddAssembly(typeof(WellKnownTestErrors).Assembly, filterByServiceErrorsAttribute: false)
            .Build()
            .BuildServiceProvider();
        var factory = services.GetRequiredService<IServiceErrorFactory>();

        // Act
        var error = factory.CreateError(
            new ServiceErrorDto
            {
                Code = WellKnownTestErrors.UserNotFound.ShortCode,
                Message = "This is a test error."
            });
        var unexpectedError = factory.CreateError(
            new ServiceErrorDto
            {
                Code = "unknown-error",
                Message = "This is a test error."
            });
        var externalError = factory.CreateError(
            new ServiceErrorDto
            {
                Scheme = "external",
                Application = "other-service",
                Code = WellKnownExternalTestErrors.ExternalInvalidAmountError.ShortCode,
                Message = "This is a test error."
            });
        var exception = factory.CreateException(error);
        var unexpectedException = factory.CreateException(unexpectedError);
        var externalException = factory.CreateException(externalError);

        // Assert
        error.Should().Be(
            new ServiceError(WellKnownTestErrors.UserNotFound, "This is a test error."));
        unexpectedError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Internal(WellKnownTestErrors.RootGroup, "unknown-error"),
                "This is a test error."));
        externalError.Should().Be(
            new ServiceError(
                WellKnownExternalTestErrors.ExternalInvalidAmountError,
                "This is a test error."));

        exception.Should().BeOfType<UserNotFoundException>();
        exception.Error.Should().Be(error);
        unexpectedException.Should().BeOfType<TestServiceException>();
        unexpectedException.Error.Should().Be(unexpectedError);
        externalException.Should().BeOfType<ExternalServiceException>();
        externalException.Error.Should().Be(externalError);
    }

    [Fact]
    public void WellKnownErrorsInAssemblyWithTypedFactory_ShouldBeExpected()
    {
        // Arrange
        using var services = new ServiceCollection()
            .AddServiceErrors<ITestErrorFactory>(WellKnownTestErrors.RootGroup)
            .AddAssemblyAndDependencies(
                typeof(WellKnownTestErrors).Assembly,
                referenceNamePrefix: "ServiceErrors.",
                filterByServiceErrorsAttribute: false)
            .Build()
            .BuildServiceProvider();
        var factory = services.GetRequiredService<ITestErrorFactory>();

        // Act
        var error = factory.CreateError(
            new ServiceErrorDto
            {
                Code = WellKnownTestErrors.UserAlreadyExists.ShortCode,
                Message = "This is a test error."
            });
        var unexpectedError = factory.CreateError(
            new ServiceErrorDto
            {
                Code = "unknown-error",
                Message = "This is a test error."
            });
        var externalError = factory.CreateError(
            new ServiceErrorDto
            {
                Scheme = "external",
                Application = "other-service",
                Code = WellKnownExternalTestErrors.ExternalInvalidAmountError.ShortCode,
                Message = "This is a test error."
            });
        var exception = factory.CreateException(error);
        var unexpectedException = factory.CreateException(unexpectedError);
        var externalException = factory.CreateException(externalError);

        // Assert
        error.Should().Be(
            new ServiceError(WellKnownTestErrors.UserAlreadyExists, "This is a test error."));
        unexpectedError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Internal(WellKnownTestErrors.RootGroup, "unknown-error"),
                "This is a test error."));
        externalError.Should().Be(
            new ServiceError(
                WellKnownExternalTestErrors.ExternalInvalidAmountError,
                "This is a test error."));

        exception.Should().BeOfType<UserAlreadyExistsException>();
        exception.Error.Should().Be(error);
        unexpectedException.Should().BeOfType<TestServiceException>();
        unexpectedException.Error.Should().Be(unexpectedError);
        externalException.Should().BeOfType<ExternalServiceException>();
        externalException.Error.Should().Be(externalError);
    }
}