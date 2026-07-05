# CodeMe.ServiceErrors

CodeMe.ServiceErrors is a library for describing service-level errors as first-class values and turning them into serializable payloads or exceptions. It is designed for APIs, background services, and distributed systems where you want a stable error contract across app boundaries.

## Minimal example

Handling the errors:
```csharp
using CodeMe.ServiceErrors;

// Error URI: problem://orders-api/orders/order-not-found, status 404
var descriptor = ErrorDescriptor.NotFound(
    group: ErrorGroupUri.Create("problem", "orders-api", "orders"),
    code: "order-not-found");

var error = new ServiceError(descriptor, "Order 66 was not found");

// ...

if (error.Matches(descriptor))
{
    // handle the error
}
```

Well-known errors, conversions and DI registration:
```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using Microsoft.Extensions.DependencyInjection;
using static WellKnownOrderApiErrors;

var services = new ServiceCollection();
services
    .AddServiceErrors(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors));

using var provider = services.BuildServiceProvider();
var errorFactory = provider.GetRequiredService<IServiceErrorFactory>();

ServiceError error = new ServiceError(OrderNotFound, "Order 42 was not found");
ServiceErrorDto dto = errorFactory.CreateDto(error);
ServiceError errorFromDto = errorFactory.CreateError(dto);
// DTO content in JSON format:
// {
//   "scheme": "problem",
//   "application": "orders-api",
//   "category": "orders",
//   "code": "order-not-found",
//   "statusCode": "NotFound",
//   "message": "Order 42 was not found"
// }

// returns OrderNotFoundException
IServiceException exception = errorFactory.CreateException(errorFromDto);
ServiceError errorFromException = exception.Error;

[ServiceErrors]
internal static class WellKnownOrderApiErrors
{
    public static readonly ErrorGroupUri RootGroup = ErrorGroupUri.Create("problem", "orders-api");

    public static readonly ErrorGroupUri OrdersGroup = RootGroup.SubGroup("orders");

    [ServiceException<OrderNotFoundException>]
    public static readonly ErrorDescriptor OrderNotFound =
        ErrorDescriptor.NotFound(OrdersGroup, "order-not-found");
}

internal sealed class OrderNotFoundException : ServiceException
{
    public OrderNotFoundException(ServiceError error) 
        : base(AssertMatches(OrderNotFound, error))
    {
    }

    public OrderNotFoundException(string message, Exception? innerException = null) 
        : base(OrderNotFound, message, innerException)
    {
    }
}
```

# Documentation

Check [documentation](https://github.com/ig-sinicyn/CodeMe) for more details and examples.