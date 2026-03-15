using DotNet.Performance.Examples.Patterns;
using FluentAssertions;

namespace DotNet.Performance.Tests.Patterns;

public sealed class CollectionsMarshalDemoTests
{
    [Fact]
    public void SumWithLinq_NullData_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => CollectionsMarshalDemo.SumWithLinq(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SumWithMarshal_NullData_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => CollectionsMarshalDemo.SumWithMarshal(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SumWithLinq_EmptyList_ReturnsZero()
    {
        // Act
        long result = CollectionsMarshalDemo.SumWithLinq(new List<int>());

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void SumWithMarshal_EmptyList_ReturnsZero()
    {
        // Act
        long result = CollectionsMarshalDemo.SumWithMarshal(new List<int>());

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 6L)]
    [InlineData(new[] { -1, 0, 1 }, 0L)]
    [InlineData(new[] { 100, 200, 300 }, 600L)]
    public void SumWithLinq_ReturnsCorrectSum(int[] items, long expected)
    {
        // Arrange
        List<int> data = new List<int>(items);

        // Act
        long result = CollectionsMarshalDemo.SumWithLinq(data);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 6L)]
    [InlineData(new[] { -1, 0, 1 }, 0L)]
    [InlineData(new[] { 100, 200, 300 }, 600L)]
    public void SumWithMarshal_ReturnsCorrectSum(int[] items, long expected)
    {
        // Arrange
        List<int> data = new List<int>(items);

        // Act
        long result = CollectionsMarshalDemo.SumWithMarshal(data);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10_000)]
    public void SumWithMarshal_MatchesSumWithLinq(int size)
    {
        // Arrange
        List<int> data = Enumerable.Range(1, size).ToList();
        long expected = CollectionsMarshalDemo.SumWithLinq(data);

        // Act
        long result = CollectionsMarshalDemo.SumWithMarshal(data);

        // Assert
        result.Should().Be(expected);
    }
}
