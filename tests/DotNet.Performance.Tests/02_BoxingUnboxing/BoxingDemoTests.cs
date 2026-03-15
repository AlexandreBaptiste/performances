using DotNet.Performance.Examples.BoxingUnboxing;
using FluentAssertions;

namespace DotNet.Performance.Tests.BoxingUnboxing;

public sealed class BoxingDemoTests
{
    private readonly BoxingDemo _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Naive_ReturnsCorrectSum(int count)
    {
        // Arrange — sum of 0..count-1 = count*(count-1)/2
        int expected = count * (count - 1) / 2;

        // Act
        int result = _sut.Naive(count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Optimized_ReturnsCorrectSumMatchingNaive(int count)
    {
        // Arrange
        int expected = _sut.Naive(count);

        // Act
        int result = _sut.Optimized(count);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Naive_ThrowsArgumentOutOfRangeException_WhenCountIsZero()
    {
        // Act
        Action act = () => _sut.Naive(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
