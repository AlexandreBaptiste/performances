using DotNet.Performance.Examples.GCRegionsAndPGO;
using FluentAssertions;

namespace DotNet.Performance.Tests.GCRegionsAndPGO;

public sealed class GCRegionsDemoTests
{
    [Fact]
    public void RunCriticalSection_NullWork_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => GCRegionsDemo.RunCriticalSection(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RunCriticalSection_ValidWork_ExecutesWork()
    {
        // Arrange
        bool wasCalled = false;

        // Act
        GCRegionsDemo.RunCriticalSection(() => wasCalled = true);

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void RunCriticalSection_WorkThrows_ExceptionPropagates()
    {
        // Act
        Action act = () => GCRegionsDemo.RunCriticalSection(
            () => throw new InvalidOperationException("test error"));

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("test error");
    }

    [Fact]
    public void RunWithoutNoGCRegion_NullWork_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => GCRegionsDemo.RunWithoutNoGCRegion(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RunWithoutNoGCRegion_ValidWork_ExecutesWork()
    {
        // Arrange
        bool wasCalled = false;

        // Act
        GCRegionsDemo.RunWithoutNoGCRegion(() => wasCalled = true);

        // Assert
        wasCalled.Should().BeTrue();
    }
}
