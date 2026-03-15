using DotNet.Performance.Examples.MemoryModel;
using FluentAssertions;

namespace DotNet.Performance.Tests.MemoryModel;

public sealed class StackVsHeapDemoTests
{
    private readonly StackVsHeapDemo _sut = new();

    [Fact]
    public void DemonstrateStackAllocation_ReturnsExpectedSum()
    {
        // Arrange — points (1,2), (3,4), (5,6) → 1+2+3+4+5+6 = 21

        // Act
        int result = _sut.DemonstrateStackAllocation();

        // Assert
        result.Should().Be(21);
    }

    [Fact]
    public void DemonstrateHeapAllocation_ReturnsHeapPoint()
    {
        // Act
        HeapPoint point = _sut.DemonstrateHeapAllocation();

        // Assert
        point.Should().NotBeNull();
        point.X.Should().Be(10);
        point.Y.Should().Be(20);
    }

    [Fact]
    public void GetTotalAllocatedBytes_ReturnsPositiveValue()
    {
        // Act
        long bytes = _sut.GetTotalAllocatedBytes();

        // Assert
        bytes.Should().BeGreaterThan(0);
    }
}
