namespace CodeMe.ServiceErrors.Abstractions.UnitTests;

public class ServiceErrorTests
{
    [Fact]
    public void ServiceError_Properties_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var ex = new Exception("test-exception");
        var error = new ServiceError(descriptor, "Some detail")
        {
            InnerException = ex
        };

        // Assert
        error.Descriptor.Should().Be(descriptor);
        error.Message.Should().Be("Some detail");
        error.InnerException.Should().Be(ex);
        error.InnerErrors.Should().BeEmpty();
    }

    [Fact]
    public void ServiceError_InnerErrors_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var innerError = new ServiceError(descriptor, "Some inner detail");
        var error = new ServiceError(descriptor, "Some detail")
        {
            InnerException = new ServiceException(innerError)
        };

        // Assert
        error.Descriptor.Should().Be(descriptor);
        error.Message.Should().Be("Some detail");
        error.InnerErrors.Should().BeEquivalentTo([innerError]);
    }

    [Fact]
    public void ServiceError_MultipleInnerErrors_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var innerError = new ServiceError(descriptor, "Some inner detail");
        var innerError2 = new ServiceError(descriptor, "Some inner detail-2");
        var error = new ServiceError(descriptor, "Some detail")
        {
            InnerException = new AggregateException(
                new ServiceException(innerError),
                new ServiceException(innerError2))
        };

        // Assert
        error.Descriptor.Should().Be(descriptor);
        error.Message.Should().Be("Some detail");
        error.InnerErrors.Should().BeEquivalentTo([innerError, innerError2]);
    }

    [Fact]
    public void ServiceError_Equals_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var otherDescriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.Internal);
        var ex = new Exception("test-exception");

        var error = new ServiceError(descriptor, "Some detail");
        var errorWithException = new ServiceError(descriptor, "Some detail")
        {
            InnerException = ex
        };
        var otherError = new ServiceError(descriptor, "Some detail-2");
        var otherDescriptorError = new ServiceError(otherDescriptor, "Some detail");

        // Assert
        error.Equals(error).Should().Be(true);
        error.Equals(errorWithException).Should().Be(true);
        error.Equals(otherError).Should().Be(false);
        error.Equals(otherDescriptorError).Should().Be(false);

        error.GetHashCode().Should().Be(error.GetHashCode());
        error.GetHashCode().Should().Be(errorWithException.GetHashCode());
        error.GetHashCode().Should().NotBe(otherError.GetHashCode());
        error.GetHashCode().Should().NotBe(otherDescriptorError.GetHashCode());
    }

    [Fact]
    public void ServiceError_Matches_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var otherDescriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.Internal);
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var error = new ServiceError(descriptor, "Some detail");

        // Assert
        error.Matches(descriptor.Type).Should().BeTrue();
        error.Matches(errorGroup).Should().BeTrue();
        error.Matches(descriptor).Should().BeTrue();
        error.Matches(otherGroup).Should().BeFalse();
        error.Matches(otherType).Should().BeFalse();
        error.Matches(otherDescriptor).Should().BeFalse();
    }

    [Fact]
    public void ServiceError_MatchesAny_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var otherDescriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.Internal);
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var error = new ServiceError(descriptor, "Some detail");

        // Assert
        error.MatchesAny(otherDescriptor.Type, descriptor.Type).Should().BeTrue();
        error.MatchesAny(otherGroup, errorGroup).Should().BeTrue();
        error.MatchesAny(otherDescriptor, descriptor).Should().BeTrue();
        error.MatchesAny(otherGroup).Should().BeFalse();
        error.MatchesAny(otherType).Should().BeFalse();
        error.MatchesAny(otherDescriptor).Should().BeFalse();
    }
}