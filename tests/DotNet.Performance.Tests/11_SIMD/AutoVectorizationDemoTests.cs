using DotNet.Performance.Examples.SIMD;
using FluentAssertions;

namespace DotNet.Performance.Tests.SIMD;

public sealed class AutoVectorizationDemoTests
{
    [Fact]
    public void VectorizationFriendlySum_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        int[] data = null!;

        // Act
        Action act = () => AutoVectorizationDemo.VectorizationFriendlySum(data);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VectorizationUnfriendlySum_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        int[] data = null!;

        // Act
        Action act = () => AutoVectorizationDemo.VectorizationUnfriendlySum(data);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VectorizationFriendlySum_EmptyArray_ReturnsZero()
    {
        // Arrange
        int[] data = [];

        // Act
        long result = AutoVectorizationDemo.VectorizationFriendlySum(data);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void VectorizationUnfriendlySum_EmptyArray_ReturnsZero()
    {
        // Arrange
        int[] data = [];

        // Act
        long result = AutoVectorizationDemo.VectorizationUnfriendlySum(data);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(100)]
    [InlineData(1000)]
    public void VectorizationUnfriendlySum_MatchesFriendlySum(int length)
    {
        // Arrange
        int[] data = Enumerable.Range(1, length).ToArray();
        long expected = AutoVectorizationDemo.VectorizationFriendlySum(data);

        // Act
        long result = AutoVectorizationDemo.VectorizationUnfriendlySum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, 10L)]
    [InlineData(new[] { -1, -2, 3 }, 0L)]
    [InlineData(new[] { 100 }, 100L)]
    public void VectorizationFriendlySum_ReturnsCorrectSum(int[] data, long expected)
    {
        // Act
        long result = AutoVectorizationDemo.VectorizationFriendlySum(data);

        // Assert
        result.Should().Be(expected);
    }
}
