using DotNet.Performance.Examples.GCRegionsAndPGO;
using FluentAssertions;

namespace DotNet.Performance.Tests.GCRegionsAndPGO;

public sealed class DynamicPGODemoTests
{
    [Theory]
    [InlineData(0, 0L)]
    [InlineData(1, 0L)]
    [InlineData(4, 14L)]   // 0 + 1 + 4 + 9
    [InlineData(5, 30L)]   // 0 + 1 + 4 + 9 + 16
    public void HotLoop_ReturnsCorrectSumOfSquares(int iterations, long expected)
    {
        // Act
        long result = DynamicPGODemo.HotLoop(iterations);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ColdMethod_Returns42()
    {
        // Act
        int result = DynamicPGODemo.ColdMethod();

        // Assert
        result.Should().Be(42);
    }
}
