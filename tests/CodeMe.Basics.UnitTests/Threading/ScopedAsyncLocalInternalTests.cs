using CodeMe.Basics.Threading;

namespace CodeMe.Basics.UnitTests.Threading;

using TestSnapshot = ScopedAsyncLocal<string>.TestSnapshot;

public class ScopedAsyncLocalInternalTests
{
    [Fact]
    public void EmptyScope_ShouldBeNull()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();
        var current = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeNull();
    }

    [Fact]
    public void Scope_BeginInitialize_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        using var scope = local.BeginScopeInitialization();
        var current = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().Be(TestSnapshot.Uninitialized);
    }

    [Fact]
    public void Scope_Initialize_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        using var scope = local.BeginScopeInitialization();
        scope.Initialize("Hello!");
        var current = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().Be("Hello!");
        current.Should().Be(new TestSnapshot("Hello!"));
    }

    [Fact]
    public void Scope_BeginInitializeDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope = local.BeginScopeInitialization();
        scope.Dispose();
        var current = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeNull();
    }

    [Fact]
    public void Scope_InitializeDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope = local.BeginScopeInitialization();
        scope.Initialize("Hello!");
        scope.Dispose();
        var current = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeNull();
    }

    [Fact]
    public void Scope_BeginScope_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        using var scope = local.BeginScope("Hello!");
        var current = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().Be("Hello!");
        current.Should().Be(new TestSnapshot("Hello!"));
    }

    [Fact]
    public void Scope_BeginScopeDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope = local.BeginScope("Hello!");
        scope.Dispose();
        var current = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeNull();
    }

    [Fact]
    public async Task Scope_BeginScopeAsync_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act

        using var scope = await local.BeginScopeAsync(
            async () =>
            {
                await Task.Delay(1);
                return "Hello!";
            });
        var current = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().Be("Hello!");
        current.Should().Be(new TestSnapshot("Hello!"));
    }

    [Fact]
    public async Task Scope_BeginScopeAsyncDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope = await local.BeginScopeAsync(
            async () =>
            {
                await Task.Delay(1);
                return "Hello!";
            });
        scope.Dispose();
        var current = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeNull();
    }

    [Fact]
    public void NestedScopes_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        using var scope1 = local.BeginScope("Hello!");
        using var scope2 = local.BeginScope("Hello from nested scope!");
        var current = internals.GetSnapshots();

        // Assert
        local.Current.Should().Be("Hello from nested scope!");
        current.Should().BeEquivalentTo(
        [
            new TestSnapshot("Hello!"),
            new TestSnapshot("Hello from nested scope!")
        ]);
    }

    [Fact]
    public void NestedScopes_SingleDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        using var scope1 = local.BeginScope("Hello!");
        var scope2 = local.BeginScope("Hello from nested scope!");
        scope2.Dispose();
        var current = internals.GetSnapshots();

        // Assert
        local.Current.Should().Be("Hello!");
        current.Should().BeEquivalentTo(
        [
            new TestSnapshot("Hello!")
        ]);
    }

    [Fact]
    public void NestedScopes_OutOfOrderDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope1 = local.BeginScope("Hello!");
        using var scope2 = local.BeginScope("Hello from nested scope!");
        scope1.Dispose();
        var current = internals.GetSnapshots();

        // Assert
        local.Current.Should().Be("Hello from nested scope!");
        current.Should().BeEquivalentTo(
        [
            TestSnapshot.Disposed,
            new TestSnapshot("Hello from nested scope!")
        ]);
    }

    [Fact]
    public void NestedScopes_FullDispose_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();

        // Act
        var scope1 = local.BeginScope("Hello!");
        var scope2 = local.BeginScope("Hello from nested scope!");
        scope1.Dispose();
        scope2.Dispose();
        var current = internals.GetSnapshots();

        // Assert
        local.Current.Should().BeNull();
        current.Should().BeEmpty();
    }

    [Fact]
    public async Task BeginScopeAsync_FullAsyncPath_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        var beforeCall = internals.GetSnapshot();

        var inInit = TestSnapshot.Disposed;
        var task = local.BeginScopeAsync(
            async () =>
            {
                await completion.Task;
                inInit = internals.GetSingleSnapshot();
                return "Hello!";
            });
        var afterCall = internals.GetSingleSnapshot();

        completion.SetResult();
        var scope = await task;
        var afterAwait = internals.GetSingleSnapshot();

        scope.Dispose();
        var afterDispose = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        beforeCall.Should().BeNull();
        inInit.Should().Be(TestSnapshot.Uninitialized);
        afterCall.Should().Be(TestSnapshot.Uninitialized);
        afterAwait.Should().Be(new TestSnapshot("Hello!"));
        afterDispose.Should().BeNull();
    }

    [Fact]
    public async Task BeginScopeAsyncAndFail_FullAsyncPath_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        var beforeCall = internals.GetSnapshot();

        var inInit = TestSnapshot.Disposed;
        var task = local.BeginScopeAsync(
            async () =>
            {
                await completion.Task;
                inInit = internals.GetSingleSnapshot();
                throw new InvalidOperationException();
            });
        var afterCall = internals.GetSingleSnapshot();

        completion.SetResult();
        await Assert.ThrowsAsync<InvalidOperationException>(() => task);
        var afterAwait = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().BeNull();
        beforeCall.Should().BeNull();
        inInit.Should().Be(TestSnapshot.Uninitialized);
        afterCall.Should().Be(TestSnapshot.Uninitialized);
        afterAwait.Should().Be(TestSnapshot.Disposed);
    }

    [Fact]
    public async Task DisposeScopeAsync_ShouldBeExpected()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        var scope = local.BeginScope("Hello!");
        var beforeDispose = internals.GetSingleSnapshot();

        var inAsyncDispose = TestSnapshot.Uninitialized;
        var inAsyncAfterDispose = (TestSnapshot?)TestSnapshot.Uninitialized;
        var task = Task.Run(
            async () =>
            {
                await completion.Task;
                inAsyncDispose = internals.GetSingleSnapshot();
                scope.Dispose();
                inAsyncAfterDispose = internals.GetSnapshot();
            },
            TestContext.Current.CancellationToken);
        var afterCall = internals.GetSingleSnapshot();

        completion.SetResult();
        await task;
        var afterAwait = internals.GetSingleSnapshot();

        // Assert
        local.Current.Should().BeNull();
        beforeDispose.Should().Be(new TestSnapshot("Hello!"));
        inAsyncDispose.Should().Be(new TestSnapshot("Hello!"));
        inAsyncAfterDispose.Should().BeNull();
        afterCall.Should().Be(new TestSnapshot("Hello!"));
        afterAwait.Should().Be(TestSnapshot.Disposed);
    }

    [Fact]
    public async Task DisposedScopes_ShouldBeRemoved()
    {
        // Arrange
        var local = new ScopedAsyncLocal<string>();
        var internals = local.GetTestAccessor();
        var disposedAsyncScope = local.BeginScope("Hello!");
        await Task.Run(
            async () =>
            {
                await Task.Delay(1);
                disposedAsyncScope.Dispose();
            },
            TestContext.Current.CancellationToken);

        // Act
        var beforeNewScope = internals.GetSingleSnapshot();
        var scope = local.BeginScope("Hello-2!");
        var afterNewScope = internals.GetSingleSnapshot();
        scope.Dispose();
        var afterDispose = internals.GetSnapshot();

        // Assert
        local.Current.Should().BeNull();
        beforeNewScope.Should().Be(TestSnapshot.Disposed);
        afterNewScope.Should().Be(new TestSnapshot("Hello-2!"));
        afterDispose.Should().BeNull();
    }
}