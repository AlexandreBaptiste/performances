using DotNet.Performance.Examples.Inlining;
using FluentAssertions;

namespace DotNet.Performance.Tests.Inlining;

public sealed class AggressiveInliningDemoTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 4)]
    [InlineData(2, 9)]
    [InlineData(5, 36)]
    [InlineData(-1, 0)]
    public void Naive_ReturnsCorrectResult(int x, int expected)
    {
        // Act
        int result = AggressiveInliningDemo.Naive(x);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 4)]
    [InlineData(2, 9)]
    [InlineData(5, 36)]
    [InlineData(-1, 0)]
    public void Optimized_ReturnsCorrectResult(int x, int expected)
    {
        // Act
        int result = AggressiveInliningDemo.Optimized(x);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-5)]
    [InlineData(100)]
    public void Optimized_MatchesNaive(int x)
    {
        // Arrange
        int expected = AggressiveInliningDemo.Naive(x);

        // Act
        int result = AggressiveInliningDemo.Optimized(x);

        // Assert
        result.Should().Be(expected);
    }
}
