# CodeMe.ServiceErrors.Abstractions

CodeMe.ServiceErrors.Abstractions helps you describe service-level errors as first-class values. The package contains contract types for the main `CodeMe.ServiceErrors` package.

A simple example:

```csharp
using CodeMe.ServiceErrors;

var descriptor = ErrorDescriptor.NotFound(
	ErrorGroupUri.Create("problem", "orders-api", "orders"),
	"order-not-found");

var error = new ServiceError(descriptor, "Order 42 was not found");
```

For more details, examples, and guidance on DI registration, serialization, and exception mapping, see the full documentation:

https://github.com/ig-sinicyn/CodeMe/blob/master/docs/ServiceErrors/README.md