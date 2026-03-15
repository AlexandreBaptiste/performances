using DotNet.Performance.Examples.RefStructs;
using FluentAssertions;

namespace DotNet.Performance.Tests.RefStructs;

public sealed class ReadOnlyRefStructDemoTests
{
    [Theory]
    [InlineData("Hello, World!", 0, 5, 5)]
    [InlineData("Hello, World!", 7, 5, 5)]
    [InlineData("test", 1, 2, 2)]
    public void CreateView_ValidSlice_ReturnsViewWithCorrectLength(
        string source, int start, int length, int expectedLength)
    {
        // Act
        ReadOnlyView view = ReadOnlyRefStructDemo.CreateView(source, start, length);

        // Assert
        ReadOnlyRefStructDemo.GetLength(view).Should().Be(expectedLength);
    }

    [Fact]
    public void CreateView_NullSource_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => ReadOnlyRefStructDemo.CreateView(null!, 0, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("Hello", 0, 5, 'H', true)]
    [InlineData("Hello", 0, 5, 'z', false)]
    [InlineData("World", 0, 5, 'o', true)]
    public void Contains_ReturnsCorrectResult(
        string source, int start, int length, char target, bool expected)
    {
        // Arrange
        ReadOnlyView view = ReadOnlyRefStructDemo.CreateView(source, start, length);

        // Act
        bool result = ReadOnlyRefStructDemo.Contains(view, target);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetLength_ReturnsSliceLength()
    {
        // Arrange
        ReadOnlyView view = ReadOnlyRefStructDemo.CreateView("abcdef", 1, 3);

        // Act
        int length = ReadOnlyRefStructDemo.GetLength(view);

        // Assert
        length.Should().Be(3);
    }
}
