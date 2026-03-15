using DotNet.Performance.Examples.SpanAndMemory;
using FluentAssertions;

namespace DotNet.Performance.Tests.SpanAndMemory;

public sealed class SpanDemoTests
{
    private readonly SpanDemo _sut = new();

    [Theory]
    [InlineData("Hello, World!", 0, 5, "Hello")]
    [InlineData("Hello, World!", 7, 5, "World")]
    [InlineData("  trimmed  ", 0, 11, "trimmed")]
    public void Naive_ReturnsCorrectSubstring(string input, int start, int length, string expected)
    {
        // Act
        string result = _sut.Naive(input, start, length);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello, World!", 0, 5, "Hello")]
    [InlineData("Hello, World!", 7, 5, "World")]
    [InlineData("  trimmed  ", 0, 11, "trimmed")]
    public void Optimized_ReturnsCorrectSubstring(string input, int start, int length, string expected)
    {
        // Act
        string result = _sut.Optimized(input, start, length);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello, World!", 0, 5)]
    [InlineData("Hello, World!", 7, 5)]
    [InlineData("The quick brown fox", 4, 5)]
    public void Optimized_MatchesNaive(string input, int start, int length)
    {
        // Arrange
        string expected = _sut.Naive(input, start, length);

        // Act
        string result = _sut.Optimized(input, start, length);

        // Assert
        result.Should().Be(expected);
    }
}
