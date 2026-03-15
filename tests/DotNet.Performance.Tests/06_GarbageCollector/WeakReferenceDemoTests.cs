using DotNet.Performance.Examples.GarbageCollector;
using FluentAssertions;

namespace DotNet.Performance.Tests.GarbageCollector;

public sealed class WeakReferenceDemoTests
{
    [Fact]
    public void CreateWeakRef_NullValue_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => WeakReferenceDemo.CreateWeakRef(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryGetValue_WhileStrongReferenceExists_ReturnsTrue()
    {
        // Arrange
        string strongRef = "still alive";
        WeakReference<string> weakRef = WeakReferenceDemo.CreateWeakRef(strongRef);

        // Act
        bool result = WeakReferenceDemo.TryGetValue(weakRef, out string? value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("still alive");

        GC.KeepAlive(strongRef); // ensure strongRef is not collected during the test
    }

    [Fact]
    public void IsAlive_WhileStrongReferenceExists_ReturnsTrue()
    {
        // Arrange
        string strongRef = "alive";
        WeakReference<string> weakRef = WeakReferenceDemo.CreateWeakRef(strongRef);

        // Act
        bool alive = WeakReferenceDemo.IsAlive(weakRef);

        // Assert
        alive.Should().BeTrue();

        GC.KeepAlive(strongRef);
    }

    [Fact]
    public void TryGetValue_NullWeakRef_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => WeakReferenceDemo.TryGetValue(null!, out _);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsAlive_NullWeakRef_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => WeakReferenceDemo.IsAlive(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
