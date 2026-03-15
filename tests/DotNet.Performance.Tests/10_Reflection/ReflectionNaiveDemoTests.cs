using DotNet.Performance.Examples.Reflection;
using FluentAssertions;

namespace DotNet.Performance.Tests.Reflection;

public sealed class ReflectionNaiveDemoTests
{
    [Fact]
    public void GetValueViaReflection_Entity_ReturnsCorrectValue()
    {
        // Arrange
        SampleEntity entity = new() { Value = 42 };

        // Act
        int result = ReflectionNaiveDemo.GetValueViaReflection(entity);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void SetValueViaReflection_Entity_SetsValue()
    {
        // Arrange
        SampleEntity entity = new() { Value = 0 };

        // Act
        ReflectionNaiveDemo.SetValueViaReflection(entity, 99);

        // Assert
        entity.Value.Should().Be(99);
    }

    [Fact]
    public void SetValueViaReflection_ThenGet_RoundTripsValue()
    {
        // Arrange
        SampleEntity entity = new();
        const int expected = 123;

        // Act
        ReflectionNaiveDemo.SetValueViaReflection(entity, expected);
        int result = ReflectionNaiveDemo.GetValueViaReflection(entity);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetValueViaReflection_NullEntity_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => ReflectionNaiveDemo.GetValueViaReflection(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetValueViaReflection_NullEntity_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => ReflectionNaiveDemo.SetValueViaReflection(null!, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
