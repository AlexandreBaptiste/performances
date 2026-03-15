using DotNet.Performance.Examples.ObjectPooling;
using FluentAssertions;

namespace DotNet.Performance.Tests.ObjectPooling;

public sealed class StringBuilderPoolDemoTests
{
    [Fact]
    public void FormatWithNew_NullTemplate_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringBuilderPoolDemo.FormatWithNew(null!, 10);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FormatWithPool_NullTemplate_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringBuilderPoolDemo.FormatWithPool(null!, 10);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void FormatWithNew_ReturnsIterationsCount(int iterations)
    {
        // Act
        int result = StringBuilderPoolDemo.FormatWithNew("Item", iterations);

        // Assert
        result.Should().Be(iterations);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void FormatWithPool_ReturnsIterationsCount(int iterations)
    {
        // Act
        int result = StringBuilderPoolDemo.FormatWithPool("Item", iterations);

        // Assert
        result.Should().Be(iterations);
    }

    [Theory]
    [InlineData("Hello", 1)]
    [InlineData("Prefix", 20)]
    [InlineData("", 5)]
    public void FormatWithPool_MatchesFormatWithNew(string template, int iterations)
    {
        // Arrange
        int expected = StringBuilderPoolDemo.FormatWithNew(template, iterations);

        // Act
        int result = StringBuilderPoolDemo.FormatWithPool(template, iterations);

        // Assert
        result.Should().Be(expected);
    }
}
