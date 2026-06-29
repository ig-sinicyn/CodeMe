using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.UnitTests.Models;
using static CodeMe.ServiceErrors.UnitTests.Models.WellKnownExternalTestErrors;
using static CodeMe.ServiceErrors.UnitTests.Models.WellKnownTestErrors;

namespace CodeMe.ServiceErrors.UnitTests.Serializable;

public class ServiceErrorFactoryExceptionTests
{
    private static readonly DefaultServiceErrorFactoryOptions _defaultOptions =
        DefaultServiceErrorFactoryOptions.Create(UnknownErrorsGroup) with
        {
            ErrorCodeMapping = new Dictionary<string, ErrorDescriptor>
            {
                [UserNotFound.ShortCode] = UserNotFound
            },
            DefaultErrorFactory = error => new TestServiceException(error),
            ErrorFactories = new Dictionary<ErrorUri, Func<ServiceError, IServiceException>>
            {
                [UserNotFound.Type] = error => new UserNotFoundException(error)
            },
            ErrorGroupFactories = new Dictionary<ErrorGroupUri, Func<ServiceError, IServiceException>>
            {
                [ExternalRootGroup] = error => new ExternalServiceException(error)
            }
        };

    [Fact]
    public void CreateException_MatchByCode_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var dto = new ServiceErrorDto
        {
            Category = UserNotFound.Category,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Some detail"
        };
        var error = new ServiceError(UserNotFound, "Some detail");

        // Act
        var fromDto = factory.CreateException(dto);
        var fromError = factory.CreateException(dto);

        // Assert
        var expected = new UserNotFoundException("Some detail");

        fromDto.Should().BeOfType<UserNotFoundException>();
        fromError.Should().BeOfType<UserNotFoundException>();

        expected.Error.Should().Be(error);
        fromDto.Error.Should().Be(expected.Error);
        fromError.Error.Should().Be(expected.Error);
    }

    [Fact]
    public void CreateException_MatchByGroup_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var dto = new ServiceErrorDto
        {
            Scheme = ExternalInvalidAmountError.Scheme,
            Application = ExternalInvalidAmountError.Application,
            Category = ExternalInvalidAmountError.Category,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Some detail"
        };
        var error = new ServiceError(ExternalInvalidAmountError, "Some detail");

        // Act
        var fromDto = factory.CreateException(dto);
        var fromError = factory.CreateException(dto);

        // Assert
        fromDto.Should().BeOfType<ExternalServiceException>();
        fromError.Should().BeOfType<ExternalServiceException>();

        fromDto.Error.Should().Be(error);
        fromError.Error.Should().Be(error);
    }

    [Fact]
    public void CreateException_DefaultFactory_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var dto = new ServiceErrorDto
        {
            Category = UserAlreadyExists.Category,
            Code = UserAlreadyExists.ShortCode,
            StatusCode = UserAlreadyExists.StatusCode,
            Message = "Some detail"
        };
        var error = new ServiceError(UserAlreadyExists, "Some detail");

        // Act
        var fromDto = factory.CreateException(dto);
        var fromError = factory.CreateException(dto);

        // Assert
        fromDto.Should().BeOfType<TestServiceException>();
        fromError.Should().BeOfType<TestServiceException>();

        fromDto.Error.Should().Be(error);
        fromError.Error.Should().Be(error);
    }
}