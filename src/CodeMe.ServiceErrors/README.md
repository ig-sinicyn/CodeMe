# CodeMe.ServiceErrors

CodeMe.ServiceErrors helps you describe service-level errors as first-class values and propagate them consistently across application boundaries.

The package is designed for APIs, background services, and distributed systems where a stable error contract matters. It lets you:

- define well-known error descriptors with stable URIs and semantics;
- carry errors as `ServiceError` values in your domain code;
- serialize them to `ServiceErrorDto` payloads;
- map them to typed exceptions when needed;
- register error definitions in DI for consistent creation and hydration.

A simple example:

```csharp
using CodeMe.ServiceErrors;

var descriptor = ErrorDescriptor.NotFound(
	ErrorGroupUri.Create("problem", "orders-api", "orders"),
	"order-not-found");

var error = new ServiceError(descriptor, "Order 42 was not found");
```

For more details, examples, and guidance on DI registration, serialization, and exception mapping, see the full documentation:

https://github.com/ig-sinicyn/CodeMe/blob/feature/unit-of-work/docs/ServiceErrors/README.md
