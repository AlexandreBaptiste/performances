using DotNet.Performance.Examples.StringOptimization;
using FluentAssertions;

namespace DotNet.Performance.Tests.StringOptimization;

public sealed class SearchValuesDemoTests
{
    [Fact]
    public void IndexOfVowelNaive_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => SearchValuesDemo.IndexOfVowelNaive(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IndexOfVowelOptimized_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => SearchValuesDemo.IndexOfVowelOptimized(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("hello", 1)]   // 'e' at index 1
    [InlineData("aXbX", 0)]   // 'a' at index 0
    [InlineData("bcdf", -1)]  // no vowel
    [InlineData("", -1)]       // empty — no match
    [InlineData("AEIOU", 0)]  // uppercase vowels
    [InlineData("bcAde", 2)]  // 'A' at index 2
    public void IndexOfVowelNaive_ReturnsCorrectIndex(string input, int expected)
    {
        // Act
        int result = SearchValuesDemo.IndexOfVowelNaive(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello", 1)]
    [InlineData("aXbX", 0)]
    [InlineData("bcdf", -1)]
    [InlineData("", -1)]
    [InlineData("AEIOU", 0)]
    [InlineData("bcAde", 2)]
    public void IndexOfVowelOptimized_ReturnsCorrectIndex(string input, int expected)
    {
        // Act
        int result = SearchValuesDemo.IndexOfVowelOptimized(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("programming")]
    [InlineData("rhythm")]
    [InlineData("Hello World")]
    [InlineData("AEIOUaeiou")]
    public void IndexOfVowelOptimized_MatchesNaive(string input)
    {
        // Arrange
        int expected = SearchValuesDemo.IndexOfVowelNaive(input);

        // Act
        int result = SearchValuesDemo.IndexOfVowelOptimized(input);

        // Assert
        result.Should().Be(expected);
    }
}
