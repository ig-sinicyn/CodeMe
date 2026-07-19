# CodeMe.ServiceErrors

CodeMe.ServiceErrors is a library for describing service-level errors as first-class values and turning them into serializable payloads or exceptions. It is designed for APIs, background services, and distributed systems where a stable error contract across application boundaries is important.

## How it works

The library separates an error's identity, its semantics, and its transport shape:

1. `ErrorGroupUri` and `ErrorUri` describe a stable problem type such as `problem://orders-api/orders/order-not-found`. The URI format is compatible with [RFC 9457: Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc9457).
2. `ErrorDescriptor` adds the runtime semantics: HTTP-like status, transience, and severity.
3. `ServiceError` is the value you pass around inside your application. It combines the descriptor with a human-readable message and optional inner exception or nested errors, so the contract stays explicit without forcing every caller to use exceptions.
4. `IServiceErrorFactory` is the bridge between the in-process model and the outside world. It can convert a `ServiceError` into a serializable `ServiceErrorDto`, restore a `ServiceError` from a DTO, or create an exception for a known error.
5. `ServiceException` and `IServiceException` let you propagate the same error as an exception while preserving the underlying descriptor. When the factory knows about a registered error, it can instantiate a typed exception for that descriptor or its containing group.
6. Matching helpers (`Matches`, `MatchesAny`) compare descriptors, error groups, error URIs, and status codes so callers can branch on well-known errors without hard-coding strings.

In practice, you usually define a small set of well-known descriptors once, register them in DI, create and inspect `ServiceError` values in your domain code, and serialize them at API boundaries. That gives you a contract that is stable for tooling, readable for humans, and cheap to process in production code.

## Basic examples

Handling errors:
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

## More scenarios

### Checking for well-known errors

All error-related types provide `Matches(...)` and `MatchesAny()` methods. Matching works as follows:
* x matches a status code: exact match.
* x matches an `ErrorUri`: exact match.
* x matches an `ErrorGroupUri`: matches if `x.Group` is a descendant of the specified error group.
* x matches an `ErrorDescriptor`: matches if `x.Type` and `x.StatusCode` are equal to the descriptor's type and status code.

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

// Test for error groups (checks whether any group contains the descriptor's error group)
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

You can attach an exception type to a well-known error descriptor or error group through the `ServiceExceptionAttribute` family. When a matching error is turned into an exception, the factory instantiates the configured exception type. The target exception type must expose a public constructor that accepts `ServiceError` as its single argument.

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

If an exception is not registered, the `IServiceErrorFactory.CreateError()` method returns a `ServiceException` whose error code is derived from the exception's type.

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


### DI registrations for well-known errors

The DI extensions support three common registration patterns:

- Register a specific well-known error type.
- Register all well-known error types from one assembly.
- Register well-known error types from an assembly and its referenced assemblies.

```csharp
using CodeMe.ServiceErrors;
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

In some cases, it is useful to have a custom error factory configuration rather than the default one. For example, you may want a specialized error factory for a client of an external service without allowing those external service errors to be used across the rest of your application. Meet the typed factory concept. You must create a marker interface derived from `IServiceErrorFactory` and use it as the type argument for the `AddServiceErrors<TErrorFactory>()` call.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.DependencyInjection;
using CodeMe.ServiceErrors.Serializable;
using Microsoft.Extensions.DependencyInjection;
using static WellKnownOrderApiErrors;

var services = new ServiceCollection();
services
    .AddServiceErrors<IOrdersErrorFactory>(RootGroup)
    .Add(typeof(WellKnownOrderApiErrors));

public interface IOrdersErrorFactory : IServiceErrorFactory
{
}
```

With this setup, the container can resolve `IOrdersErrorFactory` as a typed service error factory while still using the same well-known error registration model.

### DI-free error factory

For scenarios where you do not want to use DI, you can create a service error factory directly using `DefaultServiceErrorFactoryBuilder` with the same configuration and without DI.

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
