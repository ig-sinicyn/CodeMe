# CodeMe

CodeMe is a set of small, focused, reusable libraries aimed to reduce amount of boring boilerplate code in .NET applications. 

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

## Advanced usage

### Checking for well-known errors

All error-related types do provide `Matches(...)` / `MatchesAny()` methods. Match logic works as follows:
* x matches to StatusCode: exact match.
* x matches to ErrorUri: exact match.
* x matches to ErrorGroupUri: match if x.Group is descendant of specified error group.
* x matches to ErrorDescriptor: match if x.Type and x.StatusCode are equal to descriptor's type and status code.

Example usage:
```csharp
using CodeMe.ServiceErrors;

var rootGroup = ErrorGroupUri.Create("problem", "orders-api");
var otherAppGroup = ErrorGroupUri.Create("problem", "users-api");
var notFoundErrorUri = ErrorUri.Create(rootGroup.SubGroup("orders"), "order-not-found");
var notFoundDescriptor = new ErrorDescriptor(notFoundErrorUri, ErrorStatusCode.NotFound);

var error = new ServiceError(notFoundDescriptor, "Order 404 was not found");
var exception = new ServiceException(error);

// Test for error descriptor (checks for type and status code)
if (error.Matches(notFoundDescriptor))
{
}

// Test for error groups (checks if any group do contain descriptor's error group)
if (notFoundDescriptor.MatchesAny(rootGroup, otherAppGroup))
{
}

// Test for problem type URI
try
{
}
catch (ServiceException ex) when (ex.Matches(notFoundErrorUri))
{
}

// Test for error status code
try
{
}
catch (Exception ex) 
    when (ex is IServiceException x && x.Matches(ErrorStatusCode.NotFound))
{
}
```

### Serialization

To convert between a `ServiceError` and a serializable `ServiceErrorDto`, use `IServiceErrorFactory`.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.Serializable.Builders;
using static WellKnownOrderApiErrors;

var factory = new DefaultServiceErrorFactoryBuilder(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors))
    .Build();

var serviceError = new ServiceError(OrderNotFound, "Order 404 was not found");

ServiceErrorDto dto = factory.CreateDto(serviceError);
// DTO content in JSON format:
// {
//   "scheme": "problem",
//   "application": "orders-api",
//   "category": "orders",
//   "code": "order-not-found",
//   "statusCode": "NotFound",
//   "message": "Order 404 was not found"
// }

ServiceError restored = factory.CreateError(dto);
```

### Exception Mapping

You can attach an exception type to a well-known error descriptor or error group with the `ServiceExceptionAttribute` family. When a matching error is turned into an exception, the factory instantiates the configured exception type. The target exception type must expose a public constructor that accepts `ServiceError` as a single argument.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Serializable.Builders;
using static WellKnownOrderApiErrors;

var factory = new DefaultServiceErrorFactoryBuilder(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors))
    .Build();

ServiceError serviceError = new ServiceError(OrderNotFound, "Order 42 was not found");
IServiceException exception = factory.CreateException(serviceError); // returns OrderNotFoundException

// ...

ServiceError restoredError = factory.CreateError((Exception)exception);
```

#### Unknown exceptions

If exception is not registered, the `IServiceErrorFactory.CreateError()` method will return ServiceException with error code derived from exception's type.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Serializable.Builders;
using static WellKnownOrderApiErrors;

var factory = new DefaultServiceErrorFactoryBuilder(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors))
    .Build();

var ex = new InvalidOperationException("Something strange happened");
var error = factory.CreateError(ex);
// StatusCode:     Internal
// Type:           "problem://orders-api/invalid-operation"
// Message:        "Something strange happened"
// InnerException: ex
```


### DI registrations of well-known errors

The DI extensions support three common registration patterns:

- Register a specific well-known errors type.
- Register all well-known error types from one assembly.
- Register well-known error types from an assembly and its referenced assemblies.

```csharp
using CodeMe.ServiceErrors.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using static WellKnownOrderApiErrors;

var services = new ServiceCollection();

services
    .AddServiceErrors(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors));

// or
services
    .AddServiceErrors(RootGroup)
    .AddAssembly(typeof(WellKnownOrderApiErrors).Assembly, filterByServiceErrorsAttribute: false);

services
    .AddServiceErrors(RootGroup)
    .AddAssemblyAndDependencies(
        typeof(WellKnownOrderApiErrors).Assembly,
        referenceNamePrefix: "CodeMe.",
        filterByServiceErrorsAttribute: true);
```

The `Add` overload registers the static error container type directly. `AddAssembly` scans a single assembly for static classes marked with `[ServiceErrors]` or for classes containing public static fields of well-known error types, depending on the `filterByServiceErrorsAttribute` flag. `AddAssemblyAndDependencies` walks the assembly graph and registers error definitions from matching dependencies. If `referenceNamePrefix` is specified, only root assembly and assemblies whose name starts with the prefix will be scanned.

#### Typed error factories and per-factory well-known error registration

In some cases it is useful to have a custom error factory configuration instead of the default one. As example, you may want to have a specialized error factory for client of some external service and do not want external service errors to be used across the rest of your application. Meet the typed factory concept. You have to create a marker interface derived from `IServiceErrorFactory` and use it as a type argument for the `AddServiceErrors<TErrorFactory>()` call.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using Microsoft.Extensions.DependencyInjection;
using static WellKnownOrderApiErrors;

public interface IOrdersErrorFactory : IServiceErrorFactory
{
}

var services = new ServiceCollection();
services
    .AddServiceErrors<IOrdersErrorFactory>(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors));
```

With this setup, the container can resolve `IOrdersErrorFactory` as a typed service error factory while still using the same well-known error registration model.

### DI-free error factory

For scenarios where you do not want to use DI, you can create a service error factory directly using `DefaultServiceErrorFactoryBuilder`. Same configuration, no DI.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.Serializable.Builders;
using static WellKnownOrderApiErrors;

var factory = new DefaultServiceErrorFactoryBuilder(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors))
    .Build();

var serviceError = new ServiceError(OrderNotFound, "Order 404 was not found");

ServiceErrorDto dto = factory.CreateDto(serviceError);
```
