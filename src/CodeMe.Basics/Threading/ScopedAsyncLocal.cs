using System.Collections.Immutable;
using System.ComponentModel;

namespace CodeMe.Basics.Threading;

/// <summary>
/// Provides an ambient context for a logical execution flow.
/// </summary>
/// <typeparam name="T">The type of the ambient data.</typeparam>
public sealed class ScopedAsyncLocal<T>
    where T : class
{
    /// <summary>
    /// Ambient scope instance with partial initialization support.
    /// Please DO NOT capture the instance and use <see cref="ScopedAsyncLocal{T}.Current"/> instead.
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
        /// <exception cref="ObjectDisposedException">Scope was disposed.</exception>
        /// <exception cref="InvalidOperationException">Scope was not initialized.</exception>
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
        /// <exception cref="ObjectDisposedException">Scope was disposed.</exception>
        /// <exception cref="InvalidOperationException">Scope was already initialized.</exception>
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

        /// <inheritdoc/>
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

    // A stack of scopes is kept to support nested scopes and to restore the previous ambient value when a scope
    // is disposed.
    //
    // Scopes are initialized in two steps because the value of the async-local context must be set before any async
    // calls are executed. Otherwise, an update performed after the first await would not be propagated back to the
    // caller, because the continuation would run with a copy of the parent execution context.
    private readonly AsyncLocal<ImmutableStack<Scope>> _current;
    private readonly bool _validateDisposeOrder;

    /// <summary>
    /// Creates ambient context.
    /// </summary>
    /// <param name="validateDisposeOrder">Fail for out-of-order scope dispose.</param>
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
    /// The caller MUST call <see cref="Scope.Initialize"/> to set the value
    /// and <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be a memory leak.
    /// This method cannot be called from async method because the scope value will not be propagated back to the caller.
    /// Instead, call it from synchronous part, and return task of completion part;
    /// </summary>
    /// <example>
    /// ValueTask&lt;IDisposable&gt; BeginCustomScopeAsync()\
    /// {
    /// var scope = _scopedAsyncLocal.BeginScopeInitialization();
    /// return CompleteCustomScopeAsync(scope);
    /// }
    /// async ValueTask&lt;IDisposable&gt; CompleteCustomScopeAsync(ScopedAsyncLocal&lt;string&gt;.Scope scope)
    /// {
    /// var resource = await GetResourceAsync();
    /// scope.Initialize(resource);
    /// return scope;
    /// }
    /// </example>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Scope BeginScopeInitialization()
    {
        var newScope = new Scope(this);

        PushScope(newScope);

        return newScope;
    }

    /// <summary>
    /// Begins a new scope with new ambient value.
    /// The caller MUST call <see cref="Scope.Dispose"/> or there may be a memory leak.
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
    /// IMPORTANT: If the method is called from helper method,
    /// the helper should be synchronous and SHOULD NOT contain awaits.
    /// Otherwise, updated scope value will not be propagated back to the caller of the helper method.
    /// The caller MUST call <see cref="Scope.Dispose"/> on the end of scope lifetime or there may be a memory leak.
    /// </summary>
    /// <param name="valueFactory">
    /// Async factory for the new scope ambient value. Value will be replaced with previous one on
    /// scope disposal.
    /// </param>
    /// <returns><see cref="IDisposable"/> to restore the parent scope.</returns>
    public Task<IDisposable> BeginScopeAsync(Func<ValueTask<T?>> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        var newScope = BeginScopeInitialization();
        return CompleteScopeAsync(newScope, valueFactory);
    }

    private async Task<IDisposable> CompleteScopeAsync(
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
        _current.Value = stack == null ? [newScope] : PopDisposedScopes(stack).Push(newScope);
    }

    private void PopDisposedScopes()
    {
        if (_current.Value is { } stack)
        {
            stack = PopDisposedScopes(stack);
            _current.Value = stack.IsEmpty ? null! : stack;
        }
    }

    private ImmutableStack<Scope> PopDisposedScopes(ImmutableStack<Scope> current)
    {
        var newStack = current;

        while (!newStack.IsEmpty && newStack.Peek().IsDisposed)
        {
            newStack = newStack.Pop();
        }

        return newStack;
    }
}