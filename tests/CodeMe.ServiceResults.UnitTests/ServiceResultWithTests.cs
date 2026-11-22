using CodeMe.ServiceErrors;
using static CodeMe.ServiceResults.ServiceResult;

namespace CodeMe.ServiceResults.UnitTests;

public class ServiceResultWithTests
{
    private static readonly ErrorDescriptor _testErrorDescriptor = ErrorDescriptor.Internal(
        ErrorGroupUri.Create("err", "service"),
        "error-code");

    private static readonly ServiceError _testError = new(_testErrorDescriptor, "Message");
    private static readonly ServiceError _startTestError = new(_testErrorDescriptor, "StartError");

    [Fact]
    public void WithForOk_ShouldBeExpected()
    {
        // Arrange
        var result = Ok();

        // Act
        var callCount = 0;
        var withOk = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(Ok(2));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(FromError<int>(_testError));
    }

    [Fact]
    public void WithForError_ShouldBeExpected()
    {
        // Arrange
        var result = FromError(_startTestError);

        // Act
        var callCount = 0;
        var withOk = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = result.With(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(0);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
    }

    [Fact]
    public async Task WithAsyncForOk_ShouldBeExpected()
    {
        // Arrange
        var result = Ok();

        // Act
        var callCount = 0;
        var withOk = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(Ok(2));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(FromError<int>(_testError));
    }

    [Fact]
    public async Task WithAsyncForError_ShouldBeExpected()
    {
        // Arrange
        var result = FromError(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await result.WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(0);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
    }

    [Fact]
    public async Task WithForOkTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult>(Ok());

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(Ok(2));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(FromError<int>(_testError));
    }

    [Fact]
    public async Task WithForErrorTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await resultFactory().WithAsync(
            () =>
            {
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(0);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
    }

    [Fact]
    public async Task WithAsyncForOkTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult>(Ok());

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(Ok(2));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(FromError<int>(_testError));
    }

    [Fact]
    public async Task WithAsyncForErrorTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok();
            });
        var withError = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return Ok(2);
            });
        var withGenericError = await resultFactory().WithAsync(
            async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(0);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be((ServiceResult)_startTestError);
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be((ServiceResult<int>)_startTestError);
    }
}