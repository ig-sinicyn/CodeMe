using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults.UnitTests;

public class ServiceResultOfTWithTests
{
    private static readonly ErrorDescriptor _testErrorDescriptor = ErrorDescriptor.Internal(
        ErrorGroupUri.Create("err", "service"),
        "error-code");

    private static readonly ServiceError _testError = new(_testErrorDescriptor, "Message");
    private static readonly ServiceError _startTestError = new(_testErrorDescriptor, "StartError");

    [Fact]
    public void WithForOkOfT_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.Ok(1.0);

        // Act
        var callCount = 0;
        var withOk = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = result.With(
            x =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.Ok(3));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.FromError<int>(_testError));
    }

    [Fact]
    public void WithForErrorOfT_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.FromError<double>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = result.With(
            x =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = result.With(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
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
    public async Task WithAsyncForOkOfT_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.Ok(1.0);

        // Act
        var callCount = 0;
        var withOk = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await result.WithAsync(
            async x =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.Ok(3));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.FromError<int>(_testError));
    }

    [Fact]
    public async Task WithAsyncForErrorOfT_ShouldBeExpected()
    {
        // Arrange
        var result = ServiceResult.FromError<double>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await result.WithAsync(
            async x =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await result.WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
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
    public async Task WithForOkOfTTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult<double>>(ServiceResult.Ok(1.0));

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            x =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.Ok(3));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.FromError<int>(_testError));
    }

    [Fact]
    public async Task WithForErrorOfTTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult<double>>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            x =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await resultFactory().WithAsync(
            _ =>
            {
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
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
    public async Task WithAsyncForOkOfTTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult<double>>(ServiceResult.Ok(1.0));

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            async x =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
            });

        // Assert
        callCount.Should().Be(4);
        withOk
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.Ok());
        withError
            .Should().BeOfType<ServiceResult>()
            .And.Be(ServiceResult.FromError(_testError));
        withValue
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.Ok(3));
        withGenericError
            .Should().BeOfType<ServiceResult<int>>()
            .And.Be(ServiceResult.FromError<int>(_testError));
    }

    [Fact]
    public async Task WithAsyncForErrorOfTTask_ShouldBeExpected()
    {
        // Arrange
        var resultFactory = () => new ValueTask<ServiceResult<double>>(_startTestError);

        // Act
        var callCount = 0;
        var withOk = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok();
            });
        var withError = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return _testError;
            });
        var withValue = await resultFactory().WithAsync(
            async x =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.Ok((int)x + 2);
            });
        var withGenericError = await resultFactory().WithAsync(
            async _ =>
            {
                await Task.Yield();
                Interlocked.Increment(ref callCount);
                return ServiceResult.FromError<int>(_testError);
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