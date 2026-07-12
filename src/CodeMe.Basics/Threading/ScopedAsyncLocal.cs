using System.Collections.Immutable;
using System.ComponentModel;

namespace CodeMe.Basics.Threading;

/// <summary>
/// Provides an ambient context for a logical execution flow.
/// <para>
/// You SHOULD call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be memory leaks.
/// </para>
/// Please check documentation for using scopes together with async initialization and disposal.
/// </summary>
/// <typeparam name="T">The type of the ambient data.</typeparam>
public sealed partial class ScopedAsyncLocal<T>
    where T : class
{
    // A stack of scopes is kept to support partial initialization
    // and to restore the previous ambient value when a scope is disposed.
    //
    // Scopes are initialized in two steps because AsyncLocal values are stored in current ExecutionContext.
    // Async calls create copies of ExecutionContext and any changes in derived contexts are not passed back to the parent execution context.
    // Therefore, we start new scope synchronously in the caller execution context.
    // The rest of initialization of a new scope may be performed asynchronously.
    // The new scope is available to the caller immediately, but we filter out uninitialized scopes. 
    private readonly AsyncLocal<ImmutableStack<Scope>> _current;
    private readonly bool _validateDisposeOrder;

    /// <summary>
    /// Creates ambient context.
    /// </summary>
    /// <param name="validateDisposeOrder">Fail for out-of-order scope disposal.</param>
    public ScopedAsyncLocal(bool validateDisposeOrder = false)
    {
        _current = new AsyncLocal<ImmutableStack<Scope>>();
        _validateDisposeOrder = validateDisposeOrder;
    }

    private Scope? CurrentScope
    {
        // Returns first initialized scope value or default.
        // Check the comment of the _current field for the justification.
        get
        {
            if (_current.Value is not { } stack)
            {
                return null;
            }

            foreach (var scope in stack)
            {
                if (scope.IsInitialized)
                {
                    return scope;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// The current ambient value.
    /// </summary>
    public T? Current => CurrentScope?.Value;

    /// <summary>
    /// Starts a new scope with uninitialized value.
    /// <para>
    /// This method is designed for advanced scenarios and requires some care from the caller. Consider to use
    /// <see cref="BeginScope"/> or <see cref="BeginScopeAsync"/> as they are simpler to use.
    /// </para>
    /// <para>
    /// The caller MUST call <see cref="Scope.Initialize"/> to set the value of the scope.
    /// and <see cref="Scope.Dispose"/> on the end of scope lifetime (even for non-initialized scopes) or there may be a memory
    /// leak.
    /// </para>
    /// <para>
    /// If you want to pass a new scope back to the caller of your code, you SHOULD NOT use async methods.
    /// Scopes are stored as a part of <see cref="ExecutionContext"/>.
    /// Async calls create a copy of execution context and do not pass changes back to the parent context.
    /// </para>
    /// <example>
    /// A proper implementation that will pass the new scope back to the caller of the method:
    /// <code>
    ///     ValueTask&lt;IDisposable&gt; BeginCustomScopeAsync()
    ///     {
    ///         // sync part
    ///         var scope = _scopedAsyncLocal.BeginScopeInitialization();
    ///         return CompleteCustomScopeAsync(scope);
    ///     }
    /// 
    ///     async ValueTask&lt;IDisposable&gt; CompleteCustomScopeAsync(ScopedAsyncLocal&lt;string&gt;.Scope scope)
    ///     {
    ///         // asynchronous part
    ///         var resource = await GetResourceAsync();
    ///         scope.Initialize(resource);
    ///         return scope;
    ///     }
    ///     </code>
    /// </example>
    /// </summary>
    /// <returns><see cref="Scope"/> to complete initialization and to restore the parent scope.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Scope BeginScopeInitialization()
    {
        var newScope = new Scope(this);

        PushScope(newScope);

        return newScope;
    }

    /// <summary>
    /// Begins a new scope with new ambient value.
    /// <para>
    /// The caller MUST call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be a memory leak.
    /// </para>
    /// <para>
    /// If you want to pass a new scope back to the caller of your code, you SHOULD NOT use async methods.
    /// Scopes are stored as a part of <see cref="ExecutionContext"/>.
    /// Async calls create a copy of execution context and do not pass changes back to the parent context.
    /// </para>
    /// Consider to use <see cref="BeginScopeAsync"/> or <see cref="BeginScopeInitialization"/> if you need to initialize the
    /// scope asynchronously.
    /// </summary>
    /// <param name="value">The new scope ambient value. Will be replaced with previous one on scope disposal.</param>
    /// <returns><see cref="IDisposable"/> to restore the parent scope.</returns>
    public IDisposable BeginScope(T? value)
    {
        var newScope = new Scope(this, value);

        PushScope(newScope);

        return newScope;
    }

    /// <summary>
    /// Begins a new scope with new ambient value.
    /// <para>
    /// The caller MUST call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be a memory leak.
    /// </para>
    /// <para>
    /// If you want to pass a new scope back to the caller of your code, you SHOULD NOT use async methods.
    /// Scopes are stored as a part of <see cref="ExecutionContext"/>.
    /// Async calls create a copy of execution context and do not pass changes back to the parent context.
    /// </para>
    /// <example>
    /// A proper implementation that will pass the new scope back to the caller of the method:
    /// <code>
    ///     ValueTask&lt;IDisposable&gt; BeginResourceScopeAsync()
    ///     {
    ///         // sync part
    ///         return _scopedAsyncLocal.BeginScopeAsync(()=> CreateResourceAsync());
    ///     }
    /// 
    ///     async ValueTask&lt;Resource&gt; CreateResourceAsync() { ... }
    ///     </code>
    /// </example>
    /// </summary>
    /// <param name="valueFactory">
    /// Async factory for the new scope ambient value. Value will be replaced with previous one on scope disposal.
    /// </param>
    /// <returns><see cref="IDisposable"/> to restore the parent scope.</returns>
    public Task<IDisposable> BeginScopeAsync(Func<ValueTask<T?>> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        var newScope = BeginScopeInitialization();
        return CompleteScopeAsync(newScope, valueFactory);
    }

    private static async Task<IDisposable> CompleteScopeAsync(
        Scope newScope,
        Func<ValueTask<T?>> valueFactory)
    {
        try
        {
            var value = await valueFactory();
            newScope.Initialize(value);

            return newScope;
        }
        catch (Exception)
        {
            newScope.Dispose();
            throw;
        }
    }

    private void AssertIsCurrentScope(Scope expected)
    {
        if (_validateDisposeOrder && expected is { IsInitialized: true } && !ReferenceEquals(expected, CurrentScope))
        {
            throw new InvalidOperationException(
                "Scope's Current value mismatch. Please do dispose scopes in reverse order of scope creation.");
        }
    }

    private void PushScope(Scope newScope)
    {
        var stack = _current.Value;
        _current.Value = stack == null
            ? ImmutableStack.Create(newScope)
            : PopDisposedScopes(stack).Push(newScope);
    }

    private void PopDisposedScopes()
    {
        if (_current.Value is { } stack)
        {
            stack = PopDisposedScopes(stack);
            _current.Value = stack.IsEmpty ? null! : stack;
        }
    }

    private static ImmutableStack<Scope> PopDisposedScopes(ImmutableStack<Scope> current)
    {
        var newStack = current;

        while (!newStack.IsEmpty && newStack.Peek().IsDisposed)
        {
            newStack = newStack.Pop();
        }

        return newStack;
    }

    /// <summary>
    /// Ambient scope instance with partial initialization support.
    /// You SHOULD call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be memory leaks.
    /// It is especially important if you are using scopes inside a loop or recursive calls
    /// as you may end with a very long stack of non-disposed scopes.
    /// <para>
    /// If you store scope in wrapper, it is recommended to dispose the scope synchronously.
    /// This minor optimization will remove reference to the disposed scope from calling execution context.
    /// </para>
    /// <example>
    /// A proper implementation that will clear caller's execution context reference to the disposed scope:
    /// <code>
    ///     ValueTask DisposeAsync()
    ///     {
    ///         _scope.Dispose();
    ///         return CompleteDisposeAsync();
    ///     }
    /// 
    ///     async ValueTask CompleteDisposeAsync() { ... }
    ///     </code>
    /// </example>
    /// </summary>
    public sealed class Scope : IDisposable
    {
        private readonly ScopedAsyncLocal<T> _owner;
        private T? _value;

        internal Scope(ScopedAsyncLocal<T> owner)
        {
            _owner = owner;
        }

        internal Scope(ScopedAsyncLocal<T> owner, T? value)
            : this(owner)
        {
            _value = value;
            IsInitialized = true;
        }

        /// <summary>
        /// <c>True</c> if the scope was initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Value for the initialized scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">Scope was not initialized.</exception>
        /// <exception cref="ObjectDisposedException">Scope was disposed.</exception>
        public T? Value
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, GetType());

                return IsInitialized
                    ? _value
                    : throw new InvalidOperationException("The scope value is not initialized.");
            }
        }

        /// <summary>
        /// Initializes the scope with specified value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Scope was already initialized.</exception>
        /// <exception cref="ObjectDisposedException">Scope was disposed.</exception>
        public void Initialize(T? value)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, GetType());

            if (IsInitialized)
            {
                throw new InvalidOperationException("The scope value is already initialized.");
            }

            _value = value;
            IsInitialized = true;
        }

        /// <summary>
        /// Closes current scope and restores previous one.
        /// You SHOULD call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be memory leaks.
        /// It is especially important if you are using scopes inside a loop or recursive calls
        /// as you may end with a very long stack of non-disposed scopes.
        /// <para>
        /// If you store scope in wrapper, it is recommended to dispose the scope synchronously.
        /// This minor optimization will remove reference to the disposed scope from calling execution context.
        /// </para>
        /// <example>
        /// A proper implementation that will clear caller's execution context reference to the disposed scope:
        /// <code>
        ///     ValueTask DisposeAsync()
        ///     {
        ///         _scope.Dispose();
        ///         return CompleteDisposeAsync();
        ///     }
        /// 
        ///     async ValueTask CompleteDisposeAsync() { ... }
        ///     </code>
        /// </example>
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                _owner.AssertIsCurrentScope(this);
                _value = null;
                IsInitialized = false;
                IsDisposed = true;
                _owner.PopDisposedScopes();
            }
        }
    }
}