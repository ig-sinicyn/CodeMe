using CodeMe.Threading;

namespace CodeMe.Basics.UnitTests.Threading;

public class ScopedAsyncLocalTests
{
    [Fact]
    public void EmptyScope_ShouldBeNull()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Assert
        local.Current.Should().BeNull();
    }

    [Fact]
    public void Scope_BeginDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        string? inScope;
        var before = local.Current;
        using (local.BeginScope("Hello!"))
        {
            inScope = local.Current;
        }

        var after = local.Current;

        // Assert
        before.Should().BeNull();
        inScope.Should().Be("Hello!");
        after.Should().BeNull();
    }

    [Fact]
    public void ScopeBeforeInitialization_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        using var scope = local.BeginScopeInitialization();

        // Assert
        local.Current.Should().BeNull();
        scope.IsInitialized.Should().BeFalse();
        Assert.Throws<InvalidOperationException>(() => scope.Value);
        scope.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void ScopeAfterInitialization_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        using var scope = local.BeginScopeInitialization();
        scope.Initialize("Hello!");

        // Assert
        local.Current.Should().Be("Hello!");
        scope.IsInitialized.Should().BeTrue();
        scope.Value.Should().Be("Hello!");
        scope.IsDisposed.Should().BeFalse();
        Assert.Throws<InvalidOperationException>(() => scope.Initialize("Hello!"));
    }

    [Fact]
    public void ScopeAfterInitializationAndDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        var scope = local.BeginScopeInitialization();
        scope.Initialize("Hello!");
        scope.Dispose();

        // Assert
        local.Current.Should().BeNull();
        scope.IsInitialized.Should().BeFalse();
        Assert.Throws<ObjectDisposedException>(() => scope.Value);
        scope.IsDisposed.Should().BeTrue();
        Assert.Throws<ObjectDisposedException>(() => scope.Initialize("Hello!"));
    }

    [Fact]
    public void ScopeAfterDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        var scope = local.BeginScopeInitialization();
        scope.Dispose();

        // Assert
        local.Current.Should().BeNull();
        scope.IsInitialized.Should().BeFalse();
        Assert.Throws<ObjectDisposedException>(() => scope.Value);
        scope.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Scope_NestedBeginDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        var before1 = local.Current;
        string? inScope1;
        string? inScope2;
        string? after2;
        using (local.BeginScope("Hello!"))
        {
            inScope1 = local.Current;

            using (local.BeginScope("Hello from nested scope!"))
            {
                inScope2 = local.Current;
            }

            after2 = local.Current;
        }

        var after1 = local.Current;

        // Assert
        before1.Should().BeNull();
        inScope1.Should().Be("Hello!");
        inScope2.Should().Be("Hello from nested scope!");
        after2.Should().Be("Hello!");
        after1.Should().BeNull();
    }

    [Fact]
    public async Task Scope_NestedBeginAsyncDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        var before1 = local.Current;
        string? inScope1;
        string? inScope2;
        string? after2;
        using (await BeginScopeAsync(local, "Hello!"))
        {
            inScope1 = local.Current;

            using (await BeginScopeAsync(local, "Hello from nested scope!"))
            {
                inScope2 = local.Current;
            }

            after2 = local.Current;
        }

        var after1 = local.Current;

        // Assert
        before1.Should().BeNull();
        inScope1.Should().Be("Hello!");
        inScope2.Should().Be("Hello from nested scope!");
        after2.Should().Be("Hello!");
        after1.Should().BeNull();
    }

    [Fact]
    public async Task Scope_DoesNotReturnFromAsyncCall()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act
        var before = local.Current;
        string? inScope = null;
        var scope = await Task.Run(
            async () =>
            {
                var result = local.BeginScope("Hello!");
                await Task.Delay(1);
                inScope = local.Current;
                return result;
            },
            TestContext.Current.CancellationToken);

        var after = local.Current;
        scope.Dispose();

        // Assert
        before.Should().BeNull();
        inScope.Should().Be("Hello!");
        after.Should().BeNull();
    }

    [Fact]
    public async Task ScopeDispose_AppliedFromAsyncCall()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();

        // Act

        var scope = local.BeginScope("Hello!");
        var before = local.Current;
        await Task.Run(
            async () =>
            {
                await Task.Delay(1);
                scope.Dispose();
            },
            TestContext.Current.CancellationToken);

        var after = local.Current;

        // Assert
        before.Should().Be("Hello!");
        after.Should().BeNull();
    }

    [Fact]
    public void ScopeValidation_ShouldThrow()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>(validateDisposeOrder: true);
        var scopes = new List<IDisposable>();
        foreach (var value in new[] { "A", "B", "C" })
        {
            scopes.Add(local.BeginScope(value));
        }

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => scopes[0].Dispose());
        Assert.Throws<InvalidOperationException>(() => scopes[1].Dispose());
        local.Current.Should().Be("C");
        scopes[2].Dispose();
        local.Current.Should().Be("B");
        scopes[1].Dispose();
        local.Current.Should().Be("A");
        scopes[0].Dispose();
        local.Current.Should().BeNull();
    }

    [Fact]
    public void ScopeWithoutValidation_ShouldNotThrow()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>(validateDisposeOrder: false);
        var scopes = new List<IDisposable>();
        foreach (var value in new[] { "A", "B", "C" })
        {
            scopes.Add(local.BeginScope(value));
        }

        // Act & Assert
        local.Current.Should().Be("C");
        scopes[1].Dispose();
        local.Current.Should().Be("C");
        scopes[2].Dispose();
        local.Current.Should().Be("A");
        scopes[0].Dispose();
        local.Current.Should().BeNull();
    }

    [Fact]
    public async Task ScopeValidation_ShouldThrowOnBackpropagate()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>(validateDisposeOrder: true);

        // Act & Assert
        var scope = await Task.Run(
            async () =>
            {
                var result = local.BeginScope("Hello!");
                await Task.Delay(1);
                return result;
            },
            TestContext.Current.CancellationToken);
        Assert.Throws<InvalidOperationException>(() => scope.Dispose());
    }

    private static Task<IDisposable> BeginScopeAsync(ScopedAsyncLocal<string> local, string value) =>
        local.BeginScopeAsync(
            async () =>
            {
                await Task.Delay(1);

                return value;
            });
}