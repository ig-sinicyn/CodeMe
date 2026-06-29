namespace CodeMe.ServiceErrors.Abstractions.UnitTests;

public class ErrorDescriptorTests
{
    [Fact]
    public void ErrorDescriptor_Properties_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");
        var error = ErrorDescriptor.Create(errorGroup, "error-code", ErrorStatusCode.NotFound);

        // Assert
        error.Type.Should().Be(ErrorUri.Create(errorGroup, "error-code"));
        error.StatusCode.Should().Be(ErrorStatusCode.NotFound);
        error.Transience.Should().Be(ErrorTransience.Normal);
        error.Severity.Should().Be(ErrorSeverity.ErrorResponse);

        error.Scheme.Should().Be("scheme");
        error.Application.Should().Be("app");
        error.Category.Should().Be("group");
    }

    [Fact]
    public void ErrorDescriptor_Equals_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var error = ErrorDescriptor.Create(errorGroup, "error-code", ErrorStatusCode.NotFound);
        var otherError = ErrorDescriptor.Create(otherGroup, "error-code", ErrorStatusCode.NotFound);
        var errorWithOtherSeverity = error with
        {
            Severity = ErrorSeverity.InternalError
        };

        // Assert
        error.Equals(error).Should().BeTrue();
        error.Equals(errorWithOtherSeverity).Should().BeFalse();
        error.Equals(otherError).Should().BeFalse();
        error.GetHashCode().Should().Be(error.GetHashCode());
        error.GetHashCode().Should().NotBe(errorWithOtherSeverity.GetHashCode());
        error.GetHashCode().Should().NotBe(otherError.GetHashCode());
    }

    [Fact]
    public void ErrorDescriptor_Matches_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var error = ErrorDescriptor.Create(errorGroup, "error-code", ErrorStatusCode.NotFound);
        var otherError = ErrorDescriptor.Create(otherGroup, "error-code", ErrorStatusCode.NotFound);
        var errorWithOtherSeverity = error with
        {
            Severity = ErrorSeverity.InternalError
        };
        var errorWithOtherTransience = error with
        {
            Transience = ErrorTransience.Transient
        };
        var errorWithOtherStatus = error with
        {
            StatusCode = ErrorStatusCode.InvalidArgument
        };

        // Assert
        error.Matches(error.Type).Should().BeTrue();
        error.Matches(errorGroup).Should().BeTrue();
        error.Matches(error).Should().BeTrue();
        error.Matches(otherGroup).Should().BeFalse();
        error.Matches(otherType).Should().BeFalse();
        error.Matches(otherError).Should().BeFalse();
        error.Matches(errorWithOtherSeverity).Should().BeTrue();
        error.Matches(errorWithOtherTransience).Should().BeTrue();
        error.Matches(errorWithOtherStatus).Should().BeFalse();
    }

    [Fact]
    public void ErrorDescriptor_MatchesAny_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var error = ErrorDescriptor.Create(errorGroup, "error-code", ErrorStatusCode.NotFound);
        var otherError = ErrorDescriptor.Create(otherGroup, "error-code", ErrorStatusCode.NotFound);
        var errorWithOtherSeverity = error with
        {
            Severity = ErrorSeverity.InternalError
        };
        var errorWithOtherTransience = error with
        {
            Transience = ErrorTransience.Transient
        };
        var errorWithOtherStatus = error with
        {
            StatusCode = ErrorStatusCode.InvalidArgument
        };

        // Assert
        error.MatchesAny(otherError.Type, error.Type).Should().BeTrue();
        error.MatchesAny(otherGroup, errorGroup).Should().BeTrue();
        error.MatchesAny(otherError, error).Should().BeTrue();
        error.MatchesAny(otherGroup).Should().BeFalse();
        error.MatchesAny(otherType).Should().BeFalse();
        error.MatchesAny(otherError).Should().BeFalse();
        error.MatchesAny(errorWithOtherSeverity).Should().BeTrue();
        error.MatchesAny(errorWithOtherTransience).Should().BeTrue();
        error.MatchesAny(errorWithOtherStatus).Should().BeFalse();
    }
}