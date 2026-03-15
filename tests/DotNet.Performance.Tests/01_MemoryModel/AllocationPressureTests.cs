using DotNet.Performance.Examples.MemoryModel;
using FluentAssertions;

namespace DotNet.Performance.Tests.MemoryModel;

public sealed class AllocationPressureTests
{
    private readonly AllocationPressure _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Naive_ReturnsCorrectSum(int size)
    {
        // Arrange — sum of 0..size-1 = size*(size-1)/2
        long expected = (long)size * (size - 1) / 2;

        // Act
        long result = _sut.Naive(size);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
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
    public void Optimized_ThrowsArgumentOutOfRangeException_WhenSizeExceedsBuffer()
    {
        // Act
        Action act = () => _sut.Optimized(1025);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("size");
    }

    [Fact]
    public void Naive_ThrowsArgumentOutOfRangeException_WhenSizeIsZero()
    {
        // Act
        Action act = () => _sut.Naive(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
