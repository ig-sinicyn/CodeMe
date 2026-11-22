# CodeMe.ServiceErrors.Results

CodeMe.ServiceErrors.Results provides a set of union types for representing the outcome of a service operation as a first-class value.

## Basic examples

### Success and failure

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

### Implicit conversions

A `ServiceError` or a value can be returned through implicit conversions:

```csharp
ServiceResult okResult = new OkResult();
ServiceResult result = new ServiceError(orderNotFound, "Order was not found");
ServiceResult<string> valueResult = "The Answer";
ServiceResult<string> errorResult = new ServiceError(orderNotFound, "Order was not found");
```

A `ServiceResult<T>` can be converted to `ServiceResult`.

```csharp
ServiceResult<Order> orderResult = LoadOrder();
ServiceResult result = orderResult;
```

### Explicit error and value access

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Results;

var ordersGroup = ErrorGroupUri.Create("problem", "orders-api", "orders");
var orderNotFound = ErrorDescriptor.NotFound(ordersGroup, "order-not-found");

var orders = new Dictionary<int, Order>();
ServiceResult<Order> result = GetOrder(123);

if (result.IsError)
{
    Console.WriteLine(result.Error.Value.Message);
}
else
{
    // throws a ServiceException if result.IsError is true
    Console.WriteLine(result.Value.Name);
}

ServiceResult<Order> GetOrder(int id)
{
    if (!orders.TryGetValue(id, out var order))
    {
        return ServiceResult.FromError<Order>(
            new ServiceError(orderNotFound, $"Order {id} not found"));
    }

    return ServiceResult.Ok(order);
}

internal record Order(int Id, string Name);
```

### Chaining operations

Use `With` to run the next operation only after a successful result. If the result is an error, the original error is preserved and the callback is not invoked.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Results;

var result = GetAccountId("A-100")
    .With(accountId => LoadOrder(accountId));

if (result is ServiceError error)
{
    Console.WriteLine(error.Message);
}

ServiceResult<int> GetAccountId(string accountNumber);
ServiceResult<Order> LoadOrder(int accountId);
```

### Async chaining

Use `WithAsync` for asynchronous callbacks and `ValueTask`-based service results.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Results;

var result = await GetAccountIdAsync("A-100")
    .WithAsync(x => LoadOrderAsync(x))
    .WithAsync(x => ProcessOrderAsync(x));

if (result is ServiceError error)
{
    Console.WriteLine(error.Message);
}

ValueTask<ServiceResult<int>> GetAccountIdAsync(string accountNumber) => default;
ValueTask<ServiceResult<Order>> LoadOrderAsync(int accountId) => default;
ValueTask<ServiceResult<Order>> ProcessOrderAsync(Order order) => default;

record Order(int Id, int AccountId, string Name);
```

### Pattern matching

You can switch directly over the union members:

```csharp
var message = result switch
{
    OkResult => "Success",
    ServiceError error => error.Message
};
```

For `ServiceResult<T>`:

```csharp
var message = result switch
{
    Order order => order.Name,
    ServiceError error => error.Message,
    null => "No value"
};

// For non-nulalble types:
var message = result switch
{
    Order order => order.Name,
    ServiceError error => error.Message
};
```

When a nullable or reference type has a successful `null` value, include `null` in the pattern switch to handle that success state.

### Matching errors

The `Matches` and `MatchesAny` extension methods inspect the `ServiceError` contained in a result. They work with `ErrorStatusCode`, `ErrorGroupUri`, `ErrorUri`, and `ErrorDescriptor`.

```csharp
using CodeMe.ServiceErrors;
using CodeMe.ServiceErrors.Results;

var ordersGroup = ErrorGroupUri.Create("problem", "orders-api", "orders");
var orderNotFound = ErrorUri.Create(ordersGroup, "order-not-found");

var result = ServiceResult.FromError(...);

if (result.Matches(orderNotFound))
{
    // handle the specific not-found error
}

if (result.Matches(ErrorStatusCode.NotFound))
{
    // handle a not-found status
}

if (result.MatchesAny(ErrorStatusCode.NotFound, ErrorStatusCode.AlreadyExists))
{
    // handle either status
}

if (result.Matches(ordersGroup))
{
    // handle an error belonging to the orders group
}
```

The overloads for `ServiceResult<T>` behave the same way. A successful result never matches an error value.
