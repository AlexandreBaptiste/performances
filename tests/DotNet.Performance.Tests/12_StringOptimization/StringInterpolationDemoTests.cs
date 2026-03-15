using DotNet.Performance.Examples.StringOptimization;
using FluentAssertions;

namespace DotNet.Performance.Tests.StringOptimization;

public sealed class StringInterpolationDemoTests
{
    [Fact]
    public void Naive_NullName_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringInterpolationDemo.Naive(null!, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Optimized_NullName_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringInterpolationDemo.Optimized(null!, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("Alice", 42, "Alice 42")]
    [InlineData("Bob", 0, "Bob 0")]
    [InlineData("", 100, " 100")]
    public void Naive_ReturnsCorrectString(string name, int count, string expected)
    {
        // Act
        string result = StringInterpolationDemo.Naive(name, count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Alice", 42, "Alice 42")]
    [InlineData("Bob", 0, "Bob 0")]
    [InlineData("", 100, " 100")]
    public void Optimized_ReturnsCorrectString(string name, int count, string expected)
    {
        // Act
        string result = StringInterpolationDemo.Optimized(name, count);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Alice", 1)]
    [InlineData("Bob", 99)]
    [InlineData("X", -5)]
    public void Optimized_MatchesNaive(string name, int count)
    {
        // Arrange
        string expected = StringInterpolationDemo.Naive(name, count);

        // Act
        string result = StringInterpolationDemo.Optimized(name, count);

        // Assert
        result.Should().Be(expected);
    }
}
