using DotNet.Performance.Examples.SpanAndMemory;
using FluentAssertions;

namespace DotNet.Performance.Tests.SpanAndMemory;

public sealed class StackallocDemoTests
{
    private readonly StackallocDemo _sut = new();

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
    public void Optimized_ThrowsArgumentOutOfRangeException_WhenSizeExceedsLimit()
    {
        // Act
        Action act = () => _sut.Optimized(257);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("size");
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
