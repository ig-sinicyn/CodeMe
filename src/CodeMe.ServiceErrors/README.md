# CodeMe.ServiceErrors

CodeMe.ServiceErrors is a library for describing service-level errors as first-class values and turning them into serializable payloads or exceptions. It is designed for APIs, background services, and distributed systems where you want a stable error contract across app boundaries.

## Introduction

The core model is built around a few simple concepts:

* `ServiceError` carries well-known error descriptor together with a human-friendly message and optional inner details.
* `ErrorDescriptor` describes an error with a problem type (error URI), HTTP-like status, transience, and severity.
* `ErrorUri` and `ErrorGroupUri` represent problem type URI inspired by [RFC 9457: Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc9457).
* `ServiceErrorDto` represents serializable service error format.
* `ServiceException` and `IServiceException` allow to pass service errors as exceptions.
* `IServiceErrorFactory` converts between `ServiceError`, `ServiceErrorDto`, and `IServiceException`.

Typical usage scenarios include:

* Enforcing usage of well-known domain errors.
* Exposing a predictable error contract from HTTP APIs or gRPC services.
* Registering error definitions in DI so services can create and rehydrate errors consistently.
* Passing errors across process boundaries using a serializable error payload.
* Use allocation-free typed errors instead of error codes or exceptions in performance-sensitive code.

### Minimal example

Well-known errors, testing for errors, conversions and DI registration:
```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using Microsoft.Extensions.DependencyInjection;
using static WellKnownOrderApiErrors;

// DI registration
var services = new ServiceCollection();
services
    .AddServiceErrors(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors));
using var provider = services.BuildServiceProvider();
var errorFactory = provider.GetRequiredService<IServiceErrorFactory>();

// Error return and handling
var error = new ServiceError(OrderNotFound, "Order 42 was not found");
// ...
if (error.Matches(OrderNotFound))
{
    // handle the error
}

// Error serialization and exception factory
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

// Well-known errors declaration
[ServiceErrors]
internal static class WellKnownOrderApiErrors
{
    public static readonly ErrorGroupUri RootGroup = ErrorGroupUri.Create("problem", "orders-api");

    public static readonly ErrorGroupUri OrdersGroup = RootGroup.SubGroup("orders");

    [ServiceException<OrderNotFoundException>]
    public static readonly ErrorDescriptor OrderNotFound =
        ErrorDescriptor.NotFound(OrdersGroup, "order-not-found");
}

// Typed exceptions
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

Check [documentation](https://github.com/ig-sinicyn/CodeMe/docs/ServiceErrors/README.md) for more details and examples.