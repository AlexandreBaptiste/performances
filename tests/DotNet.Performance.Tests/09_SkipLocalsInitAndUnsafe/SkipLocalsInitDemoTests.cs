using DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;
using FluentAssertions;

namespace DotNet.Performance.Tests.SkipLocalsInitAndUnsafe;

public sealed class SkipLocalsInitDemoTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(256)]
    public void Naive_ValidSize_ReturnsSumOfSequence(int size)
    {
        // Arrange
        int expected = size * (size - 1) / 2;

        // Act
        int result = SkipLocalsInitDemo.Naive(size);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(256)]
    public void Optimized_ValidSize_ReturnsSameSumAsNaive(int size)
    {
        // Arrange
        int naive = SkipLocalsInitDemo.Naive(size);

        // Act
        int optimized = SkipLocalsInitDemo.Optimized(size);

        // Assert
        optimized.Should().Be(naive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(257)]
    public void Naive_InvalidSize_ThrowsArgumentOutOfRangeException(int size)
    {
        // Act
        Action act = () => SkipLocalsInitDemo.Naive(size);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(257)]
    public void Optimized_InvalidSize_ThrowsArgumentOutOfRangeException(int size)
    {
        // Act
        Action act = () => SkipLocalsInitDemo.Optimized(size);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
