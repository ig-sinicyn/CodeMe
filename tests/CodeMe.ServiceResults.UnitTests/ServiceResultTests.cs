using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults.UnitTests;

public class ServiceResultTests
{
    private static readonly ErrorDescriptor _error = ErrorDescriptor.NotFound(
        ErrorGroupUri.Create("service", "o"),
        "not-found-error");

    private static readonly ErrorDescriptor _otherError = ErrorDescriptor.Internal(
        ErrorGroupUri.Create("service", "o"),
        "internal-error");

    [Fact]
    public void OkResult_Properties_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.Ok();
        var okResult = ServiceResult.Ok();
        var errorResult = ServiceResult.FromError(new ServiceError(_otherError, "Details"));

        // Assert
        result.IsError.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Equals(okResult).Should().BeTrue();
        result.Equals(errorResult).Should().BeFalse();
        result.GetHashCode().Should().Be(okResult.GetHashCode());
        result.GetHashCode().Should().NotBe(errorResult.GetHashCode());
    }

    [Fact]
    public void GenericOkResult_Properties_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.Ok(123);
        var okResult = ServiceResult.Ok(123);
        var otherOkResult = ServiceResult.Ok(123);
        var errorResult = ServiceResult.FromError<int>(new ServiceError(_otherError, "Details"));

        // Assert
        result.IsError.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Value.Should().Be(123);
        result.Equals(okResult).Should().BeTrue();
        result.Equals(otherOkResult).Should().BeTrue();
        result.Equals(errorResult).Should().BeFalse();
        result.GetHashCode().Should().Be(okResult.GetHashCode());
        result.GetHashCode().Should().NotBe(errorResult.GetHashCode());
    }

    [Fact]
    public void ErrorResult_Properties_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.FromError(new ServiceError(_error, "Details"));
        var okResult = ServiceResult.Ok();
        var errorResult = ServiceResult.FromError(new ServiceError(_error, "Details"));
        var otherErrorResult = ServiceResult.FromError(new ServiceError(_otherError, "Details"));

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(new ServiceError(_error, "Details"));
        result.Equals(okResult).Should().BeFalse();
        result.Equals(errorResult).Should().BeTrue();
        result.Equals(otherErrorResult).Should().BeFalse();
        result.GetHashCode().Should().NotBe(okResult.GetHashCode());
        result.GetHashCode().Should().Be(errorResult.GetHashCode());
        result.GetHashCode().Should().NotBe(otherErrorResult.GetHashCode());
    }

    [Fact]
    public void GenericErrorResult_Properties_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.FromError<string>(new ServiceError(_error, "Details"));
        var okResult = ServiceResult.Ok("Hello");
        var errorResult = ServiceResult.FromError<string>(new ServiceError(_error, "Details"));
        var otherErrorResult = ServiceResult.FromError<string>(new ServiceError(_otherError, "Details"));

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().Be(new ServiceError(_error, "Details"));
        var ex = Assert.Throws<ServiceException>(() => result.Value);
        ex.Error.Should().Be(new ServiceError(_error, "Details"));

        result.Equals(okResult).Should().BeFalse();
        result.Equals(errorResult).Should().BeTrue();
        result.Equals(otherErrorResult).Should().BeFalse();
        result.GetHashCode().Should().NotBe(okResult.GetHashCode());
        result.GetHashCode().Should().Be(errorResult.GetHashCode());
        result.GetHashCode().Should().NotBe(otherErrorResult.GetHashCode());
    }

    [Fact]
    public void GenericOkResult_Conversions_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.Ok(123);
        var okResult = ServiceResult.Ok();
        var convertedResult = (ServiceResult)result;

        // Assert
        // ReSharper disable once SuspiciousTypeConversion.Global
        result.Equals(okResult).Should().BeFalse();
        okResult.Equals(result).Should().BeTrue();
        convertedResult.Equals(okResult).Should().BeTrue();
    }

    [Fact]
    public void GenericErrorResult_Conversions_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.FromError<int>(new ServiceError(_error, "Details"));
        var errorResult = ServiceResult.FromError(new ServiceError(_error, "Details"));
        var convertedResult = (ServiceResult)result;

        // Assert
        // ReSharper disable once SuspiciousTypeConversion.Global
        result.Equals(errorResult).Should().BeFalse();
        errorResult.Equals(result).Should().BeTrue();
        convertedResult.Equals(errorResult).Should().BeTrue();
    }

    [Fact]
    public void OkResult_Switch_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok();
        var matched = okResult switch
        {
            OkResult => "ok",
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("ok");
    }

    [Fact]
    public void GenericOkResult_Switch_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok(123);
        var matched = okResult switch
        {
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("123");
    }

    [Fact]
    public void ErrorResult_Switch_ShouldBeExpected()
    {
        // Arrange
        var errorResult = ServiceResult.FromError(new ServiceError(_error, "Details"));
        var matched = errorResult switch
        {
            OkResult => "ok",
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("not-found-error");
    }

    [Fact]
    public void GenericErrorResult_Switch_ShouldBeExpected()
    {
        // Arrange
        var errorResult = ServiceResult.FromError<string>(new ServiceError(_error, "Details"));
        var matched = errorResult switch
        {
            string x => x,
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("not-found-error");
    }

    [Fact]
    public void OkResult_SwitchOverNullable_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<int?>(null);
        var matched = okResult switch
        {
            null => "null",
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("null");
    }

    [Fact]
    public void OkResult_SwitchOverNull_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<string?>(null);
        var matched = okResult switch
        {
            null => "null",
            string x => x,
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        matched.Should().Be("null");
    }
}