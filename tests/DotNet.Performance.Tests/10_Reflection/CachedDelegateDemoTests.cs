using DotNet.Performance.Examples.Reflection;
using FluentAssertions;

namespace DotNet.Performance.Tests.Reflection;

public sealed class CachedDelegateDemoTests
{
    [Fact]
    public void GetValueCached_Entity_ReturnsCorrectValue()
    {
        // Arrange
        SampleEntity entity = new() { Value = 77 };

        // Act
        int result = CachedDelegateDemo.GetValueCached(entity);

        // Assert
        result.Should().Be(77);
    }

    [Fact]
    public void GetValueCached_NullEntity_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => CachedDelegateDemo.GetValueCached(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void GetValueCached_VariousValues_ReturnsExpectedValue(int value)
    {
        // Arrange
        SampleEntity entity = new() { Value = value };

        // Act
        int result = CachedDelegateDemo.GetValueCached(entity);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void GetValueCached_ReturnsMatchingResultToDirectPropertyAccess()
    {
        // Arrange
        SampleEntity entity = new() { Value = 55 };

        // Act
        int cached = CachedDelegateDemo.GetValueCached(entity);
        int direct = entity.Value;

        // Assert
        cached.Should().Be(direct);
    }
}
