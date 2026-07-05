using System.Collections.Immutable;

namespace CodeMe.Basics.Threading;

/// <summary>
/// Basic building block for ambient contexts. Based on <see cref="AsyncLocal{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the ambient data.</typeparam>
public sealed class ScopedAsyncLocal<T>
    where T : class
{
    /// <summary>
    /// Async local scope lifetime.
    /// The caller MUST call <see cref="Scope.Dispose"/> or there may be a memory leak.
    /// </summary>
    internal sealed class Scope : IDisposable
    {
        private readonly ScopedAsyncLocal<T> _owner;

        private T? _value;

        public Scope(ScopedAsyncLocal<T> owner)
        {
            _owner = owner;
        }

        public Scope(ScopedAsyncLocal<T> owner, T? value)
            : this(owner)
        {
            _value = value;
            IsInitialized = true;
        }

        public bool IsInitialized { get; private set; }

        public bool IsDisposed { get; private set; }

        public T? Value
        {
            get
            {
                ObjectDisposedException.ThrowIf(IsDisposed, GetType());

                if (!IsInitialized)
                {
                    throw new InvalidOperationException("The scope value is not initialized.");
                }

                return _value;
            }
        }

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

    /// <summary>
    /// Design decisions:
    /// 1. We do support asynchronous initialization.
    /// 2. On initialization there is no way to update AsyncLocal's value
    /// for the calling method after first await
    /// as the continuation is being run using a copy of parent execution context.
    /// So, we have to store AsyncLocal's value before initialization.
    /// 3. We cannot revert store operation for the parent context so there may be cases
    /// when we leave parent context AsyncLocal's value in non-initialized state.
    /// 4. It seems the only viable option is to store stack of scopes,
    /// to perform cleanup in begin / end scope methods
    /// and to take first initialized scope in the Current accessor.
    /// </summary>
    private readonly AsyncLocal<ImmutableStack<Scope>> _current;

    private readonly bool _validateDisposeOrder;

    /// <summary>
    /// Creates ambient context
    /// </summary>
    /// <param name="validateDisposeOrder">Fail for out-of-order scope dispose.</param>
    public ScopedAsyncLocal(bool validateDisposeOrder = false)
    {
        _current = new AsyncLocal<ImmutableStack<Scope>>();
        _validateDisposeOrder = validateDisposeOrder;
    }

    /// <summary>
    /// The current ambient value.
    /// </summary>
    public T? Current => CurrentScope?.Value;

    /// <summary>
    /// The current ambient scope.
    /// </summary>
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
    /// The caller MUST call <see cref="Scope.Dispose"/> or there may be a memory leak.
    /// </summary>
    /// <param name="valueFactory">
    /// Async factory for the new scope ambient value. Value will be replaced with previous one on
    /// scope disposal.
    /// </param>
    /// <returns><see cref="IDisposable"/> to restore the parent scope.</returns>
    public async Task<IDisposable> BeginScopeAsync(Func<ValueTask<T>> valueFactory)
    {
        var newScope = new Scope(this);

        PushScope(newScope);

        try
        {
            var value = await valueFactory();
            newScope.Initialize(value);
        }
        catch (Exception ex)
        {
            newScope.Dispose();
            throw;
        }

        return newScope;
    }

    private void PushScope(Scope newScope)
    {
        PopDisposedScopes();
        _current.Value = _current.Value is { } stack
            ? stack.Push(newScope)
            : [newScope];
    }

    private void AssertIsCurrentScope(Scope expected)
    {
        if (_validateDisposeOrder && expected is { IsInitialized: true } && !ReferenceEquals(expected, CurrentScope))
        {
            throw new InvalidOperationException(
                "Scope's Current value mismatch. Please do dispose scopes in reverse order of scope creation.");
        }
    }

    private void PopDisposedScopes()
    {
        // Check the comment of the _current field for the justification.
        if (_current.Value is not { } stack)
        {
            return;
        }

        var originalStack = stack;
        while (!stack.IsEmpty && stack.Peek().IsDisposed) stack = stack.Pop();

        if (!ReferenceEquals(stack, originalStack))
        {
            _current.Value = stack;
        }
    }
}