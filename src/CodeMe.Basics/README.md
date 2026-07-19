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

`ScopedAsyncLocal<T>` also supports asynchronous flows and nested scopes. For more details and additional examples, see the full documentation:

- https://github.com/ig-sinicyn/CodeMe/blob/master/docs/Basics/AsyncLocal/README.md
- https://github.com/ig-sinicyn/CodeMe/blob/master/docs/Basics/AsyncLocal/CustomScope.md
