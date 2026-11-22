using CodeMe.ServiceErrors.Results;

namespace CodeMe.ServiceErrors.UnitTests;

public class ServiceExceptionTests
{
    [Fact]
    public void ServiceError_Properties_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var ex = new Exception("test-exception");
        var exception = new ServiceException(descriptor, "Some detail", ex);

        // Assert
        exception.Descriptor.Should().Be(descriptor);
        exception.Message.Should().Be("Some detail");
        exception.InnerException.Should().Be(ex);
        exception.InnerErrors.Should().BeEmpty();
    }

    [Fact]
    public void ServiceError_InnerErrors_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var innerError = new ServiceError(descriptor, "Some inner detail");
        var exception = new ServiceException(descriptor, "Some detail", new ServiceException(innerError));

        // Assert
        exception.Descriptor.Should().Be(descriptor);
        exception.Message.Should().Be("Some detail");
        exception.InnerErrors.Should().BeEquivalentTo([innerError]);
    }

    [Fact]
    public void ServiceError_MultipleInnerErrors_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var innerError = new ServiceError(descriptor, "Some inner detail");
        var innerError2 = new ServiceError(descriptor, "Some inner detail-2");
        var exception = new ServiceException(
            descriptor,
            "Some detail",
            new AggregateException(
                new ServiceException(innerError),
                new ServiceException(innerError2)));

        // Assert
        exception.Descriptor.Should().Be(descriptor);
        exception.Message.Should().Be("Some detail");
        exception.InnerErrors.Should().BeEquivalentTo([innerError, innerError2]);
    }

    [Fact]
    public void ServiceException_Matches_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var otherDescriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.Internal);
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var exception = new ServiceException(descriptor, "Some detail");

        // Assert
        exception.Matches(descriptor.Type).Should().BeTrue();
        exception.Matches(errorGroup).Should().BeTrue();
        exception.Matches(descriptor).Should().BeTrue();
        exception.Matches(otherGroup).Should().BeFalse();
        exception.Matches(otherType).Should().BeFalse();
        exception.Matches(otherDescriptor).Should().BeFalse();
    }

    [Fact]
    public void ServiceException_MatchesAny_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("service", "errors");
        var descriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.NotFound);
        var otherDescriptor = ErrorDescriptor.Create(errorGroup, "not-found-error", ErrorStatusCode.Internal);
        var otherGroup = ErrorGroupUri.Create("scheme", "app", "group-2");
        var otherType = ErrorUri.Create(errorGroup, "error-code-2");
        var exception = new ServiceException(descriptor, "Some detail");

        // Assert
        exception.MatchesAny(otherDescriptor.Type, descriptor.Type).Should().BeTrue();
        exception.MatchesAny(otherGroup, errorGroup).Should().BeTrue();
        exception.MatchesAny(otherDescriptor, descriptor).Should().BeTrue();
        exception.MatchesAny(otherGroup).Should().BeFalse();
        exception.MatchesAny(otherType).Should().BeFalse();
        exception.MatchesAny(otherDescriptor).Should().BeFalse();
    }

    public async Task<ServiceResult> ProcessOrderAsync(int orderId)
    {
        ServiceResult<Order> order = await _orders.GetAsync(orderId);
        if (order is ServiceError error)
        {
            return error;
        }

        var updatedOrder = order.Value with
        {
            Status = OrderStatus.Completed
        };

        var updateResult = await _orders.UpdateAsync(updatedOrder);
        return updateResult;
    }

    public async Task<ServiceResult> ProcessOrderV2Async(int orderId) =>
        await _orders.GetAsync(orderId) switch
        {
            Order order => await _orders.UpdateAsync(
                order with
                {
                    Status = OrderStatus.Completed
                }),
            ServiceError error => error
        };

    public ValueTask<ServiceResult> ProcessOrderV3Async(int orderId) =>
        _orders.GetAsync(orderId)
            .WithAsync(
                order => _orders.UpdateAsync(
                    order with
                    {
                        Status = OrderStatus.Completed
                    }));

    public record Order(int Id, string Name, OrderStatus Status)
    {
    }

    public enum OrderStatus
    {
        Ok,
        Completed
    }

    private readonly IOrdersClient _orders = default!;

    public interface IOrdersClient
    {
        ValueTask<ServiceResult<Order>> GetAsync(int id);
        ValueTask<ServiceResult> UpdateAsync(Order order);
    }
}