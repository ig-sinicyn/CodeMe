
# ScopedAsyncLocal<T>

`ScopedAsyncLocal<T>` provides ambient context for a logical execution flow. It is useful when a value should be available without explicitly passing it through every method call. Typical scenarios include unit-of-work scopes, request correlation identifiers, and tenant information.

## How it works

`ScopedAsyncLocal<T>` is built on top of `AsyncLocal<T>` and maintains a stack of scopes for the current [execution flow](https://learn.microsoft.com/en-us/dotnet/api/system.threading.executioncontext). Each scope represents a logical boundary, such as a request, a unit of work, or a tenant context. You can create multiple instances of `ScopedAsyncLocal<T>`, and each instance tracks its own value independently.

When you call `BeginScope` or `BeginScopeAsync`, a new scope is pushed onto the current execution context, and `Current` resolves to the value from the topmost initialized scope. If you call these methods from an async C# method, the new scope is stored in a copy of the execution context and will not be available to the caller of that async method. See [the example](#passing-scope-to-external-code) below for more details.

It is important to dispose scopes when they are no longer needed. A leaked scope will not be collected until the end of its execution context lifetime. In long-running code, tight loops, or deeply nested async flows, leaving scopes undisposed can lead to noticeable memory leaks.

The recommended approach is to use the `using` keyword with the scope returned by `BeginScope...`. If you want to store a scope as a field, there is a [detailed example](CustomScope.md) for custom scope wrappers.

To make leaked-scope detection easier, you can construct `ScopedAsyncLocal<T>` with `validateDisposeOrder: true` to detect out-of-order scope disposal.

## Main scenario

Use `ScopedAsyncLocal<T>` when a value should be available for the current logical execution path and reverted automatically when the scope ends. The value is visible to nested scopes and is restored to the previous ambient value when the scope is disposed.

```csharp
using CodeMe.Threading;

var context = new ScopedAsyncLocal<string>();

using (context.BeginScope("request-1"))
{
    Console.WriteLine(context.Current); // request-1

    using (context.BeginScope("nested"))
    {
        Console.WriteLine(context.Current); // nested
    }

    Console.WriteLine(context.Current); // request-1
}

Console.WriteLine(context.Current); // null
```

## Passing scope to external code

If you want to pass a new scope back to the parent method, you should call the `Begin...` method without changing the current execution context. To do so, do not mark your methods as async. Async methods run on a copy of the parent context, so the new scope will not be accessible by the parent method.

For asynchronous initialization, place the initialization code in the callback passed to `BeginScopeAsync`.

```csharp
using CodeMe.Threading;

var context = new ScopedAsyncLocal<string>();
var id = Guid.Parse("7b90489c-7d81-42bc-99d6-ba6dc118375f");

// External code
using (await BeginCustomScopeAsync(id))
{
    Console.WriteLine(context.Current); // 7b90489c-7d81-42bc-99d6-ba6dc118375f
}

Console.WriteLine(context.Current); // null

// Your helper
ValueTask<IDisposable> BeginCustomScopeAsync(Guid userId)
{
    // The method MUST be synchronous
    return context.BeginScopeAsync(async () => await GetUserStateAsync(userId));
}

Task<string> GetUserStateAsync(Guid userId) => Task.FromResult(userId.ToString());
```

For advanced scenarios, there is a `BeginScopeInitialization`/`Initialize` two-step pattern. Its primary purpose is to create a custom scope, and it requires some care from the caller. See the [custom scope example](CustomScope.md) for more details.

```csharp
using CodeMe.Threading;

var context = new ScopedAsyncLocal<string>();
var id = Guid.Parse("7b90489c-7d81-42bc-99d6-ba6dc118375f");

using (await BeginCustomScopeAsync(id))
{
    Console.WriteLine(context.Current); // 7b90489c-7d81-42bc-99d6-ba6dc118375f
}

Console.WriteLine(context.Current); // null

ValueTask<IDisposable> BeginCustomScopeAsync(Guid userId)
{
    // The method MUST be synchronous
    var scope = context.BeginScopeInitialization();
    return CompleteBeginCustomScopeAsync(scope, userId);
}

async ValueTask<IDisposable> CompleteBeginCustomScopeAsync(ScopedAsyncLocal<string>.Scope scope, Guid userId)
{
    // Asynchronous part
    try
    {
        var state = await GetUserStateAsync(userId);
        scope.Initialize(state);
        return scope;
    }
    catch (Exception)
    {
        scope.Dispose();
        throw;
    }
}

Task<string> GetUserStateAsync(Guid userId) => Task.FromResult(userId.ToString());
```
