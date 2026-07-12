
# What is included?

- CodeMe.ServiceErrors — describe service-level errors as first-class values, carry them as `ServiceError`, serialize them to DTOs, and map them to typed exceptions. See the [full documentation](docs/ServiceErrors/README.md).
- CodeMe.Basics — small infrastructure helpers such as `ScopedAsyncLocal<T>` for ambient execution context and logical scopes. See the [full documentation](docs/Basics/AsyncLocal/README.md).

# CodeMe.Basics

CodeMe.Basics provides small, reusable infrastructure types that complement the BCL.

## ScopedAsyncLocal<T>

`ScopedAsyncLocal<T>` lets you carry ambient context through a logical execution flow without explicitly passing it through every method call. Use `ScopedAsyncLocal<T>` for values such as request IDs, unit-of-work state, or tenant information. The value is automatically restored when the scope is disposed.

```csharp
using CodeMe.Threading;

var context = new ScopedAsyncLocal<string>();

using (context.BeginScope("request-1"))
{
	Console.WriteLine(context.Current); // request-1
}

Console.WriteLine(context.Current); // null
```

`ScopedAsyncLocal<T>` also supports asynchronous flows and nested scopes. For more details and additional examples, see the [full documentation](docs/Basics/AsyncLocal/README.md).

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

For more details, examples, and guidance on DI registration, serialization, and exception mapping, see the [full documentation](docs/ServiceErrors/README.md).