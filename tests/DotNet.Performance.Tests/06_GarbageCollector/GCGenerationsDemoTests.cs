using DotNet.Performance.Examples.GarbageCollector;
using FluentAssertions;

namespace DotNet.Performance.Tests.GarbageCollector;

public sealed class GCGenerationsDemoTests
{
    [Fact]
    public void GetCollectionCounts_ReturnsNonNegativeCounts()
    {
        // Act
        (int gen0, int gen1, int gen2) = GCGenerationsDemo.GetCollectionCounts();

        // Assert
        gen0.Should().BeGreaterThanOrEqualTo(0);
        gen1.Should().BeGreaterThanOrEqualTo(0);
        gen2.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void AllocateShortLivedObjects_ReturnsNonNegativeDelta()
    {
        // Act
        int delta = GCGenerationsDemo.AllocateShortLivedObjects(100);

        // Assert — we only verify the return type is logical, not the exact count (non-deterministic)
        delta.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    public void AllocateLongLivedObjects_RetainsRequestedCount(int count)
    {
        // Act
        int retained = GCGenerationsDemo.AllocateLongLivedObjects(count);

        // Assert — the list must have held exactly `count` items before being cleared
        retained.Should().Be(count);
    }
}
