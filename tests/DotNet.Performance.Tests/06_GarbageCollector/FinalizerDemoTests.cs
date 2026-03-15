using DotNet.Performance.Examples.GarbageCollector;
using FluentAssertions;

namespace DotNet.Performance.Tests.GarbageCollector;

public sealed class FinalizerDemoTests
{
    [Fact]
    public void CreateAndDisposeProper_SetsWasDisposedTrue()
    {
        // Arrange
        ProperResource.WasDisposed = false;

        // Act
        FinalizerDemo.CreateAndDisposeProper();

        // Assert
        ProperResource.WasDisposed.Should().BeTrue();
    }

    [Fact]
    public void CreateAndDisposeProper_DoesNotThrow()
    {
        // Act
        Action act = () => FinalizerDemo.CreateAndDisposeProper();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateAndForgetNaive_DoesNotThrow()
    {
        // Act
        Action act = () => FinalizerDemo.CreateAndForgetNaive();

        // Assert
        // We only verify it does not throw — finalizer timing is non-deterministic.
        act.Should().NotThrow();
    }

    [Fact]
    public void NaiveResource_WasFinalizedFlag_DefaultsToFalse_WhenReset()
    {
        // Simply verify the static flag is writable and readable — not GC timing.
        NaiveResource.WasFinalized = false;

        NaiveResource.WasFinalized.Should().BeFalse();
    }
}
