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

Service results:
```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Results;

var ordersGroup = ErrorGroupUri.Create("problem", "orders-api", "orders");
var orderNotFound = ErrorDescriptor.NotFound(ordersGroup, "order-not-found");

var orders = new Dictionary<int, Order>();
var result = GetOrder(123);
var message = result switch
{
    Order x => x.Name,
    ServiceError error => error.Message
};
Console.WriteLine(message);

ServiceResult<Order> GetOrder(int id) => orders.TryGetValue(id, out var order)
    ? order
    : new ServiceError(orderNotFound, $"Order {id} not found");

internal record Order(int Id, string Name);
```

For more details, examples, and guidance on DI registration, serialization, and exception mapping, see the full documentation:

https://github.com/ig-sinicyn/CodeMe/blob/master/docs/ServiceErrors/README.md