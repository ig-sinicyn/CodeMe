# CodeMe.ServiceErrors

CodeMe.ServiceErrors.Abstractions is a library for describing service-level errors as first-class values and turning them into serializable payloads or exceptions. It is designed for APIs, background services, and distributed systems where you want a stable error contract across app boundaries.

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

# Documentation

Check [documentation](https://github.com/ig-sinicyn/CodeMe/docs/ServiceErrors/README.md) for more details and examples.