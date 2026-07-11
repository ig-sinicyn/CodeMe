using System.Collections.Immutable;

namespace CodeMe.Basics.Threading;

public partial class ScopedAsyncLocal<T>
{
    internal TestAccessor GetTestAccessor() => new(this);

    internal readonly record struct TestSnapshot(T? Value, bool IsInitialized, bool IsDisposed)
    {
        public static TestSnapshot Uninitialized => new(null, false, false);
        public static TestSnapshot Disposed => new(null, false, true);

        public TestSnapshot(Scope scope)
            : this(
                scope.IsInitialized ? scope.Value : null,
                scope.IsInitialized,
                scope.IsDisposed)
        {
        }

        public TestSnapshot(T value)
            : this(
                value,
                true,
                false)
        {
        }
    }

    internal readonly struct TestAccessor(ScopedAsyncLocal<T> owner)
    {
        private ImmutableStack<Scope> Scopes =>
            owner._current.Value switch
            {
                null => [],
                { IsEmpty: true } => throw new InvalidOperationException("Inner stack should be null"),
                var x => x
            };

        public TestSnapshot? GetSnapshot() =>
            Scopes.Select(x => (TestSnapshot?)new TestSnapshot(x)).FirstOrDefault();

        public TestSnapshot GetSingleSnapshot() => Scopes.Select(x => new TestSnapshot(x)).Single();

        public TestSnapshot[] GetSnapshots() => Scopes.Select(x => new TestSnapshot(x)).ToArray();
    }
}