using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.UnitTests.Models;
using static CodeMe.ServiceErrors.UnitTests.Models.WellKnownExternalTestErrors;
using static CodeMe.ServiceErrors.UnitTests.Models.WellKnownTestErrors;

namespace CodeMe.ServiceErrors.UnitTests.Serializable;

public class ServiceErrorFactoryDtoTests
{
    private static readonly DefaultServiceErrorFactoryOptions _defaultOptions =
        DefaultServiceErrorFactoryOptions.Create(UnknownErrorsGroup) with
        {
            ErrorCodeMapping = new Dictionary<string, ErrorDescriptor>
            {
                [UserNotFound.ShortCode] = UserNotFound
            }
        };

    [Fact]
    public void CreateDto_FillIfUnknown_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var knownError = new ServiceError(UserNotFound, "Test user not found");
        var unexpectedError = new ServiceError(UnexpectedError, "Test user not found");
        var externalError = new ServiceError(ExternalInvalidAmountError, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);
        var unexpectedDto = factory.CreateDto(unexpectedError);
        var externalDto = factory.CreateDto(externalError);

        // Assert
        knownDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = null,
                Application = null,
                Category = null,
                Code = UserNotFound.ShortCode,
                StatusCode = UserNotFound.StatusCode,
                Message = knownError.Message
            });

        unexpectedDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = null,
                Application = null,
                Category = UnexpectedError.Category,
                Code = UnexpectedError.ShortCode,
                StatusCode = UnexpectedError.StatusCode,
                Message = unexpectedError.Message
            });

        externalDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = ExternalInvalidAmountError.Scheme,
                Application = ExternalInvalidAmountError.Application,
                Category = ExternalInvalidAmountError.Category,
                Code = ExternalInvalidAmountError.ShortCode,
                StatusCode = ExternalInvalidAmountError.StatusCode,
                Message = externalError.Message
            });
    }

    [Fact]
    public void CreateError_FillIfUnknown_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var knownDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = null,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = UnexpectedError.Category,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = ExternalInvalidAmountError.Scheme,
            Application = ExternalInvalidAmountError.Application,
            Category = ExternalInvalidAmountError.Category,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should().Be(new ServiceError(UnexpectedError, unexpectedDto.Message));
        externalError.Should().Be(new ServiceError(ExternalInvalidAmountError, externalDto.Message));
    }

    [Fact]
    public void CreateDto_FillAlways_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Always,
                ApplicationFillMode = ErrorDtoFillMode.Always,
                CategoryFillMode = ErrorDtoFillMode.Always
            });
        var knownError = new ServiceError(UserNotFound, "Test user not found");
        var unexpectedError = new ServiceError(UnexpectedError, "Test user not found");
        var externalError = new ServiceError(ExternalInvalidAmountError, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);
        var unexpectedDto = factory.CreateDto(unexpectedError);
        var externalDto = factory.CreateDto(externalError);

        // Assert
        knownDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = UserNotFound.Scheme,
                Application = UserNotFound.Application,
                Category = UserNotFound.Category,
                Code = UserNotFound.ShortCode,
                StatusCode = UserNotFound.StatusCode,
                Message = knownError.Message
            });

        unexpectedDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = UnexpectedError.Scheme,
                Application = UnexpectedError.Application,
                Category = UnexpectedError.Category,
                Code = UnexpectedError.ShortCode,
                StatusCode = UnexpectedError.StatusCode,
                Message = unexpectedError.Message
            });

        externalDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = ExternalInvalidAmountError.Scheme,
                Application = ExternalInvalidAmountError.Application,
                Category = ExternalInvalidAmountError.Category,
                Code = ExternalInvalidAmountError.ShortCode,
                StatusCode = ExternalInvalidAmountError.StatusCode,
                Message = externalError.Message
            });
    }

    [Fact]
    public void CreateError_FillAlways_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Always,
                ApplicationFillMode = ErrorDtoFillMode.Always,
                CategoryFillMode = ErrorDtoFillMode.Always
            });
        var knownDto = new ServiceErrorDto
        {
            Scheme = UserNotFound.Scheme,
            Application = UserNotFound.Application,
            Category = UserNotFound.Category,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = UnexpectedError.Scheme,
            Application = UnexpectedError.Application,
            Category = UnexpectedError.Category,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = ExternalInvalidAmountError.Scheme,
            Application = ExternalInvalidAmountError.Application,
            Category = ExternalInvalidAmountError.Category,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should().Be(new ServiceError(UnexpectedError, unexpectedDto.Message));
        externalError.Should().Be(new ServiceError(ExternalInvalidAmountError, externalDto.Message));
    }

    [Fact]
    public void CreateDto_FillNever_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Newer,
                ApplicationFillMode = ErrorDtoFillMode.Newer,
                CategoryFillMode = ErrorDtoFillMode.Newer
            });
        var knownError = new ServiceError(UserNotFound, "Test user not found");
        var unexpectedError = new ServiceError(UnexpectedError, "Test user not found");
        var externalError = new ServiceError(ExternalInvalidAmountError, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);
        var unexpectedDto = factory.CreateDto(unexpectedError);
        var externalDto = factory.CreateDto(externalError);

        // Assert
        knownDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = null,
                Application = null,
                Category = null,
                Code = UserNotFound.ShortCode,
                StatusCode = UserNotFound.StatusCode,
                Message = knownError.Message
            });

        unexpectedDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = null,
                Application = null,
                Category = null,
                Code = UnexpectedError.ShortCode,
                StatusCode = UnexpectedError.StatusCode,
                Message = unexpectedError.Message
            });

        externalDto.Should().BeEquivalentTo(
            new ServiceErrorDto
            {
                Scheme = null,
                Application = null,
                Category = null,
                Code = ExternalInvalidAmountError.ShortCode,
                StatusCode = ExternalInvalidAmountError.StatusCode,
                Message = externalError.Message
            });
    }

    [Fact]
    public void CreateError_FillNever_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Newer,
                ApplicationFillMode = ErrorDtoFillMode.Newer,
                CategoryFillMode = ErrorDtoFillMode.Newer
            });
        var knownDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = null,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = null,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = null,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should()
            .Be(
                new ServiceError(
                    ErrorDescriptor.Create(UnknownErrorsGroup, unexpectedDto.Code, unexpectedDto.StatusCode),
                    unexpectedDto.Message));
        externalError.Should()
            .Be(
                new ServiceError(
                    ErrorDescriptor.Create(UnknownErrorsGroup, externalDto.Code, externalDto.StatusCode),
                    externalDto.Message));
    }

    [Fact]
    public void CreateError_SchemeOnly_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Newer,
                ApplicationFillMode = ErrorDtoFillMode.Newer,
                CategoryFillMode = ErrorDtoFillMode.Newer
            });
        var knownDto = new ServiceErrorDto
        {
            Scheme = UserNotFound.Scheme,
            Application = null,
            Category = null,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = UnexpectedError.Scheme,
            Application = null,
            Category = null,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = ExternalInvalidAmountError.Scheme,
            Application = null,
            Category = null,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should()
            .Be(
                new ServiceError(
                    ErrorDescriptor.Create(UnknownErrorsGroup, unexpectedDto.Code, unexpectedDto.StatusCode),
                    unexpectedDto.Message));
        externalError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Create(
                    ErrorGroupUri.Create(ExternalInvalidAmountError.Scheme, RootGroup.Application),
                    externalDto.Code,
                    externalDto.StatusCode),
                externalDto.Message));
    }

    [Fact]
    public void CreateError_AppOnly_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Newer,
                ApplicationFillMode = ErrorDtoFillMode.Newer,
                CategoryFillMode = ErrorDtoFillMode.Newer
            });
        var knownDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = UserNotFound.Application,
            Category = null,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = UnexpectedError.Application,
            Category = null,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = ExternalInvalidAmountError.Application,
            Category = null,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should()
            .Be(
                new ServiceError(
                    ErrorDescriptor.Create(UnknownErrorsGroup, unexpectedDto.Code, unexpectedDto.StatusCode),
                    unexpectedDto.Message));
        externalError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Create(
                    ErrorGroupUri.Create(RootGroup.Scheme, ExternalInvalidAmountError.Application),
                    externalDto.Code,
                    externalDto.StatusCode),
                externalDto.Message));
    }

    [Fact]
    public void CreateError_CategoryOnly_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(
            _defaultOptions with
            {
                SchemeFillMode = ErrorDtoFillMode.Newer,
                ApplicationFillMode = ErrorDtoFillMode.Newer,
                CategoryFillMode = ErrorDtoFillMode.Newer
            });
        var knownDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = UserNotFound.Category,
            Code = UserNotFound.ShortCode,
            StatusCode = UserNotFound.StatusCode,
            Message = "Test user not found"
        };
        var unexpectedDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = UnexpectedError.Category,
            Code = UnexpectedError.ShortCode,
            StatusCode = UnexpectedError.StatusCode,
            Message = "Test user not found"
        };
        var externalDto = new ServiceErrorDto
        {
            Scheme = null,
            Application = null,
            Category = ExternalInvalidAmountError.Category,
            Code = ExternalInvalidAmountError.ShortCode,
            StatusCode = ExternalInvalidAmountError.StatusCode,
            Message = "Test user not found"
        };

        // Act
        var knownError = factory.CreateError(knownDto);
        var unexpectedError = factory.CreateError(unexpectedDto);
        var externalError = factory.CreateError(externalDto);

        // Assert
        knownError.Should().Be(new ServiceError(UserNotFound, knownDto.Message));
        unexpectedError.Should().Be(new ServiceError(UnexpectedError, unexpectedDto.Message));
        externalError.Should().Be(
            new ServiceError(
                ErrorDescriptor.Create(
                    ErrorGroupUri.Create(RootGroup.Scheme, RootGroup.Application, ExternalInvalidAmountError.Category),
                    externalDto.Code,
                    externalDto.StatusCode),
                externalDto.Message));
    }

    [Fact]
    public void CreateError_FromServiceException_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var exception = new ServiceException(UserNotFound, "Some detail");

        // Act
        var error = factory.CreateError(exception);

        // Assert
        error.Should().Be(exception.Error);
        error.Should().Be(new ServiceError(UserNotFound, "Some detail"));
    }

    [Fact]
    public void CreateError_FromDerivedFromServiceException_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var exception = new UserNotFoundException("Some detail");

        // Act
        var error = factory.CreateError(exception);

        // Assert
        error.Should().Be(exception.Error);
        error.Should().Be(new ServiceError(UserNotFound, "Some detail"));
    }

    [Fact]
    public void CreateError_FromCustomServiceException_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var exception = new UserAlreadyExistsException("Some detail");

        // Act
        var error = factory.CreateError(exception);

        // Assert
        error.Should().Be(exception.Error);
        error.Should().Be(new ServiceError(UserAlreadyExists, "Some detail"));
    }

    [Fact]
    public void CreateError_FromNormalException_ShouldBeExpected()
    {
        // Arrange
        var factory = new DefaultServiceErrorFactory(_defaultOptions);
        var exception = new InvalidOperationException("Some detail");

        // Act
        var error = factory.CreateError(exception);

        // Assert
        error.Should().Be(
            new ServiceError(
                ErrorDescriptor.Internal(
                    UnknownErrorsGroup,
                    "invalid-operation"),
                "Some detail"));
    }
}