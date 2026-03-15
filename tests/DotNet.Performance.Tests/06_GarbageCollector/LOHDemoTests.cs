using DotNet.Performance.Examples.GarbageCollector;
using FluentAssertions;

namespace DotNet.Performance.Tests.GarbageCollector;

public sealed class LOHDemoTests
{
    [Fact]
    public void AllocateLargeObject_ReturnsLohThresholdBytes()
    {
        // Act
        int length = LOHDemo.AllocateLargeObject();

        // Assert
        length.Should().Be(LOHDemo.LohThresholdBytes);
    }

    [Fact]
    public void AllocateSmallObject_Returns1000()
    {
        // Act
        int length = LOHDemo.AllocateSmallObject();

        // Assert
        length.Should().Be(1_000);
    }

    [Theory]
    [InlineData(84_999, false)]
    [InlineData(85_000, true)]
    [InlineData(100_000, true)]
    [InlineData(0, false)]
    public void IsOnLoh_ReturnsCorrectResult(int sizeInBytes, bool expected)
    {
        // Act
        bool result = LOHDemo.IsOnLoh(sizeInBytes);

        // Assert
        result.Should().Be(expected);
    }
}
