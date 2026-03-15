using DotNet.Performance.Examples.SpanAndMemory;
using FluentAssertions;

namespace DotNet.Performance.Tests.SpanAndMemory;

public sealed class MemoryDemoTests
{
    private readonly MemoryDemo _sut = new();

    private static readonly int[] SampleData = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

    [Theory]
    [InlineData(0, 5, 10L)]  // 0+1+2+3+4 = 10
    [InlineData(5, 5, 35L)]  // 5+6+7+8+9 = 35
    [InlineData(0, 10, 45L)] // 0..9 = 45
    public void SliceWithArray_ReturnsCorrectSum(int start, int length, long expected)
    {
        // Act
        long result = _sut.SliceWithArray(SampleData, start, length);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 5, 10L)]
    [InlineData(5, 5, 35L)]
    [InlineData(0, 10, 45L)]
    public void SliceWithMemory_ReturnsCorrectSum(int start, int length, long expected)
    {
        // Act
        long result = _sut.SliceWithMemory(SampleData, start, length);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(3, 4)]
    [InlineData(0, 10)]
    public void SliceWithMemory_MatchesSliceWithArray(int start, int length)
    {
        // Arrange
        long expected = _sut.SliceWithArray(SampleData, start, length);

        // Act
        long result = _sut.SliceWithMemory(SampleData, start, length);

        // Assert
        result.Should().Be(expected);
    }
}
