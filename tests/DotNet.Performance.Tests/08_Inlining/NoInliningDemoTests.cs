using DotNet.Performance.Examples.Inlining;
using FluentAssertions;

namespace DotNet.Performance.Tests.Inlining;

public sealed class NoInliningDemoTests
{
    [Fact]
    public void ValidateInput_NullInput_ThrowsArgumentException()
    {
        // Act
        Action act = () => NoInliningDemo.ValidateInput(null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateInput_EmptyInput_ThrowsArgumentException()
    {
        // Act
        Action act = () => NoInliningDemo.ValidateInput(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("world")]
    [InlineData(" ")]
    public void ValidateInput_ValidInput_ReturnsTrue(string input)
    {
        // Act
        bool result = NoInliningDemo.ValidateInput(input);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("hello", 5)]
    [InlineData("world!", 6)]
    [InlineData("a", 1)]
    public void ProcessInput_ValidInput_ReturnsLength(string input, int expectedLength)
    {
        // Act
        int result = NoInliningDemo.ProcessInput(input);

        // Assert
        result.Should().Be(expectedLength);
    }

    [Fact]
    public void ProcessInput_NullInput_ThrowsArgumentException()
    {
        // Act
        Action act = () => NoInliningDemo.ProcessInput(null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ProcessInput_EmptyInput_ThrowsArgumentException()
    {
        // Act
        Action act = () => NoInliningDemo.ProcessInput(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
