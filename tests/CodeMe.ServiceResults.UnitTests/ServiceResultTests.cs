using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults.UnitTests;

public class ServiceResultTests
{
    private static readonly ErrorDescriptor _error = ErrorDescriptor.NotFound(
        ErrorGroupUri.Create("app-error", "error"),
        "test-error");

    [Fact]
    public void Ok_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok();
        var passed = okResult switch
        {
            OkResult => "ok",
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("ok");
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_ShouldBeExpected()
    {
        // Arrange
        var failedResult = ServiceResult.FromError(new ServiceError(_error, "aaa"));
        var failed = failedResult switch
        {
            OkResult => "ok",
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        failed.Should().Be("test-error");
        failedResult.IsError.Should().BeTrue();
        failedResult.Error.Should().Be(new ServiceError(_error, "aaa"));
    }

    [Fact]
    public void Ok_Generic_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok(123);
        var passed = okResult switch
        {
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("123");
        okResult.Value.Should().Be(123);
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_String_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok("123");
        var passed = okResult switch
        {
            string x => x,
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("123");
        okResult.Value.Should().Be("123");
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_NullableInt32_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<int?>(123);
        var passed = okResult switch
        {
            null => "null",
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("123");
        okResult.Value.Should().Be(123);
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_NullableString_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<string?>("123");
        var passed = okResult switch
        {
            null => "null",
            string x => x,
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("123");
        okResult.Value.Should().Be("123");
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_NullInt32_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<int?>(null);
        var passed = okResult switch
        {
            null => "null",
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("null");
        okResult.Value.Should().Be(null);
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_NullString_ShouldBeExpected()
    {
        // Arrange
        var okResult = ServiceResult.Ok<string?>(null);
        var passed = okResult switch
        {
            null => "null",
            string x => x,
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        passed.Should().Be("null");
        okResult.Value.Should().Be(null);
        okResult.IsError.Should().BeFalse();
        okResult.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_Generic_ShouldBeExpected()
    {
        // Arrange
        var failedResult = ServiceResult.FromError<int>(new ServiceError(_error, "aaa"));
        var failed = failedResult switch
        {
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        failed.Should().Be("test-error");
        var ex = Assert.Throws<ServiceException>(() => failedResult.Value);
        failedResult.IsError.Should().BeTrue();
        failedResult.Error.Should().Be(new ServiceError(_error, "aaa"));
        ex.Error.Should().Be(new ServiceError(_error, "aaa"));
    }

    [Fact]
    public void Failed_NullableInt32_ShouldBeExpected()
    {
        // Arrange
        var failedResult = ServiceResult.FromError<int?>(new ServiceError(_error, "aaa"));
        var failed = failedResult switch
        {
            null => "null",
            int x => x.ToString(),
            ServiceError x => x.Descriptor.ShortCode
        };

        // Assert
        failed.Should().Be("test-error");
        var ex = Assert.Throws<ServiceException>(() => failedResult.Value);
        failedResult.IsError.Should().BeTrue();
        failedResult.Error.Should().Be(new ServiceError(_error, "aaa"));
        ex.Error.Should().Be(new ServiceError(_error, "aaa"));
    }
}