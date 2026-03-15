using System.Collections.Frozen;
using DotNet.Performance.Examples.GCRegionsAndPGO;
using FluentAssertions;

namespace DotNet.Performance.Tests.GCRegionsAndPGO;

public sealed class FrozenCollectionsDemoTests
{
    private static readonly Dictionary<string, int> SampleData = new()
    {
        ["apple"]  = 1,
        ["banana"] = 2,
        ["cherry"] = 3,
    };

    [Fact]
    public void CreateFrozenDictionary_ContainsExpectedKeys()
    {
        // Act
        FrozenDictionary<string, int> frozen = FrozenCollectionsDemo.CreateFrozenDictionary(SampleData);

        // Assert
        frozen.Should().ContainKey("apple").And.ContainKey("banana").And.ContainKey("cherry");
    }

    [Fact]
    public void CreateFrozenDictionary_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => FrozenCollectionsDemo.CreateFrozenDictionary(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateFrozenHashSet_ContainsExpectedItems()
    {
        // Arrange
        string[] items = ["one", "two", "three"];

        // Act
        FrozenSet<string> frozen = FrozenCollectionsDemo.CreateFrozenHashSet(items);

        // Assert
        frozen.Should().Contain("one").And.Contain("two").And.Contain("three");
    }

    [Fact]
    public void CreateFrozenHashSet_NullInput_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => FrozenCollectionsDemo.CreateFrozenHashSet(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("apple", 1)]
    [InlineData("banana", 2)]
    [InlineData("missing", 0)]
    public void LookupNaive_ReturnsExpectedValue(string key, int expected)
    {
        // Act
        int result = FrozenCollectionsDemo.LookupNaive(SampleData, key);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("apple", 1)]
    [InlineData("banana", 2)]
    [InlineData("missing", 0)]
    public void LookupFrozen_ReturnsExpectedValue(string key, int expected)
    {
        // Arrange
        FrozenDictionary<string, int> frozen = FrozenCollectionsDemo.CreateFrozenDictionary(SampleData);

        // Act
        int result = FrozenCollectionsDemo.LookupFrozen(frozen, key);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("apple")]
    [InlineData("banana")]
    [InlineData("cherry")]
    public void LookupFrozen_MatchesLookupNaive(string key)
    {
        // Arrange
        FrozenDictionary<string, int> frozen = FrozenCollectionsDemo.CreateFrozenDictionary(SampleData);

        // Act
        int naive  = FrozenCollectionsDemo.LookupNaive(SampleData, key);
        int frozen_ = FrozenCollectionsDemo.LookupFrozen(frozen, key);

        // Assert
        frozen_.Should().Be(naive);
    }
}
