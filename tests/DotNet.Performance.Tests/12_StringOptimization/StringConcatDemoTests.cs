using DotNet.Performance.Examples.StringOptimization;
using FluentAssertions;

namespace DotNet.Performance.Tests.StringOptimization;

public sealed class StringConcatDemoTests
{
    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "Hello")]
    [InlineData(3, "HelloHelloHello")]
    public void Naive_ReturnsCorrectString(int count, string expected)
    {
        // Act
        string result = StringConcatDemo.Naive(count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "Hello")]
    [InlineData(3, "HelloHelloHello")]
    public void Optimized_ReturnsCorrectString(int count, string expected)
    {
        // Act
        string result = StringConcatDemo.Optimized(count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public void Optimized_MatchesNaive(int count)
    {
        // Arrange
        string expected = StringConcatDemo.Naive(count);

        // Act
        string result = StringConcatDemo.Optimized(count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Naive_LengthIsCountTimesHelloLength(int count)
    {
        // Act
        string result = StringConcatDemo.Naive(count);

        // Assert
        result.Length.Should().Be(count * "Hello".Length);
    }
}
