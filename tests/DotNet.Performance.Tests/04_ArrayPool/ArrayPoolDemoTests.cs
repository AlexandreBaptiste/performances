using DotNet.Performance.Examples.ArrayPool;
using FluentAssertions;

namespace DotNet.Performance.Tests.ArrayPool;

public sealed class ArrayPoolDemoTests
{
    private readonly ArrayPoolDemo _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(256)]
    public void Naive_ReturnsCorrectSum(int size)
    {
        // Arrange
        long expected = ComputeExpectedSum(size);

        // Act
        long result = _sut.Naive(size);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(256)]
    public void Optimized_ReturnsCorrectSumMatchingNaive(int size)
    {
        // Arrange
        long expected = _sut.Naive(size);

        // Act
        long result = _sut.Optimized(size);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Naive_ThrowsArgumentOutOfRangeException_WhenSizeIsZero()
    {
        // Act
        Action act = () => _sut.Naive(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Optimized_ThrowsArgumentOutOfRangeException_WhenSizeIsZero()
    {
        // Act
        Action act = () => _sut.Optimized(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static long ComputeExpectedSum(int size)
    {
        long sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += (byte)(i % 256);
        }

        return sum;
    }
}
