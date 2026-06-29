using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.UnitTests.Models;
using static CodeMe.ServiceErrors.UnitTests.Models.WellKnownTestErrors;

namespace CodeMe.ServiceErrors.UnitTests.DependencyInjection;

public class EmittedFactoryTests
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
    public void CreateDefault_ShouldBeExpected()
    {
        // Arrange
        var factoryCallback = ServiceErrorFactoryEmitter.CreateCallback<IServiceErrorFactory>();
        var factory = factoryCallback(() => _defaultOptions);
        var knownError = new ServiceError(UserNotFound, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);

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
    }

    [Fact]
    public void CreateDefaultCustomImplementation_ShouldBeExpected()
    {
        // Arrange
        var factoryCallback = ServiceErrorFactoryEmitter
            .CreateCallback<IServiceErrorFactory>(typeof(TestErrorFactoryBase));
        var factory = factoryCallback(() => _defaultOptions);
        var knownError = new ServiceError(UserNotFound, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);

        // Assert
        factory.Should().BeAssignableTo<TestErrorFactoryBase>();
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
    }

    [Fact]
    public void CreateTwice_ShouldBeExpected()
    {
        // Arrange
        var callback = ServiceErrorFactoryEmitter.CreateCallback<IServiceErrorFactory>();
        var callback2 = ServiceErrorFactoryEmitter.CreateCallback<IServiceErrorFactory>();

        var derivedCallback = ServiceErrorFactoryEmitter.CreateCallback<ITestErrorFactory>();
        var derivedCallback2 = ServiceErrorFactoryEmitter.CreateCallback<ITestErrorFactory>();

        // Assert
        ReferenceEquals(callback, callback2).Should().BeTrue();
        ReferenceEquals(derivedCallback, derivedCallback2).Should().BeTrue();
    }

    [Fact]
    public void CreateDerived_ShouldBeExpected()
    {
        // Arrange
        var factoryCallback = ServiceErrorFactoryEmitter.CreateCallback<ITestErrorFactory>();
        var factory = factoryCallback(() => _defaultOptions);
        var knownError = new ServiceError(UserNotFound, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);

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
    }

    [Fact]
    public void CreateCustomImplementation_ShouldBeExpected()
    {
        // Arrange
        var factoryCallback = ServiceErrorFactoryEmitter
            .CreateCallback<ITestErrorFactory>(typeof(TestErrorFactoryBase));
        var factory = factoryCallback(() => _defaultOptions);
        var knownError = new ServiceError(UserNotFound, "Test user not found");

        // Act
        var knownDto = factory.CreateDto(knownError);

        // Assert
        factory.Should().BeAssignableTo<TestErrorFactoryBase>();
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
    }
}