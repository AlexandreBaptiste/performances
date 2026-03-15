using DotNet.Performance.Examples.Inlining;
using FluentAssertions;

namespace DotNet.Performance.Tests.Inlining;

public sealed class AggressiveOptimizationDemoTests
{
    [Fact]
    public void NaiveSum_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => AggressiveOptimizationDemo.NaiveSum(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void OptimizedSum_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => AggressiveOptimizationDemo.OptimizedSum(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new int[] { }, 0L)]
    [InlineData(new int[] { 1 }, 1L)]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, 15L)]
    [InlineData(new int[] { -1, -2, 3 }, 0L)]
    public void NaiveSum_ReturnsCorrectSum(int[] data, long expected)
    {
        // Act
        long result = AggressiveOptimizationDemo.NaiveSum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(new int[] { }, 0L)]
    [InlineData(new int[] { 1 }, 1L)]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, 15L)]
    [InlineData(new int[] { -1, -2, 3 }, 0L)]
    public void OptimizedSum_ReturnsCorrectSum(int[] data, long expected)
    {
        // Act
        long result = AggressiveOptimizationDemo.OptimizedSum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 })]
    [InlineData(new int[] { 10, 20, 30, 40 })]
    public void OptimizedSum_MatchesNaiveSum(int[] data)
    {
        // Arrange
        long expected = AggressiveOptimizationDemo.NaiveSum(data);

        // Act
        long result = AggressiveOptimizationDemo.OptimizedSum(data);

        // Assert
        result.Should().Be(expected);
    }
}
