using CodeMe.Threading;

namespace CodeMe.Basics.UnitTests.Threading;

public class CustomAsyncLocalScopeTests
{
    [Fact]
    public void EmptyScope_ShouldBeNull()
    {
        // Arrange
        var context = new ResourceManager();

        // Assert
        context.Current.Should().BeNull();
    }

    [Fact]
    public async Task Scope_BeginDispose_ShouldBeExpected()
    {
        // Arrange
        var context = new ResourceManager();

        // Act
        string? inScope;
        var before = context.Current?.Value;
        await using (await context.BeginScopeAsync("Hello!"))
        {
            inScope = context.Current?.Value;
        }

        var after = context.Current?.Value;

        // Assert
        before.Should().BeNull();
        inScope.Should().Be("Hello!");
        after.Should().BeNull();
    }

    private sealed class ResourceScope : IAsyncDisposable
    {
        internal IDisposable? Scope { get; set; }

        public string Value { get; set; } = null!;

        public ValueTask DisposeAsync()
        {
            Scope?.Dispose();
            return DisposeCoreAsync();
        }

        private async ValueTask DisposeCoreAsync()
        {
            await Task.Delay(1);
            Value = null!;
        }
    }

    private sealed class ResourceManager
    {
        private readonly ScopedAsyncLocal<ResourceScope> _context = new(validateDisposeOrder: true);

        public ResourceScope? Current => _context.Current;

        public ValueTask<ResourceScope> BeginScopeAsync(string value)
        {
            var scope = _context.BeginScopeInitialization();
            return BeginScopeAsyncCore(scope, value);
        }

        private static async ValueTask<ResourceScope> BeginScopeAsyncCore(
            ScopedAsyncLocal<ResourceScope>.Scope scope,
            string value)
        {
            await Task.Delay(1);
            var result = new ResourceScope
            {
                Scope = scope,
                Value = value
            };
            scope.Initialize(result);
            return result;
        }
    }
}