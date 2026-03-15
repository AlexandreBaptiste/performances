using DotNet.Performance.Examples.Reflection;
using FluentAssertions;

namespace DotNet.Performance.Tests.Reflection;

public sealed class ExpressionTreeDemoTests
{
    [Fact]
    public void GetValueViaExpression_Entity_ReturnsCorrectValue()
    {
        // Arrange
        SampleEntity entity = new() { Value = 55 };

        // Act
        int result = ExpressionTreeDemo.GetValueViaExpression(entity);

        // Assert
        result.Should().Be(55);
    }

    [Fact]
    public void GetValueViaExpression_NullEntity_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => ExpressionTreeDemo.GetValueViaExpression(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100)]
    public void GetValueViaExpression_VariousValues_ReturnsExpectedValue(int value)
    {
        // Arrange
        SampleEntity entity = new() { Value = value };

        // Act
        int result = ExpressionTreeDemo.GetValueViaExpression(entity);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void GetValueViaExpression_ReturnsMatchingResultToDirectPropertyAccess()
    {
        // Arrange
        SampleEntity entity = new() { Value = 777 };

        // Act
        int expression = ExpressionTreeDemo.GetValueViaExpression(entity);
        int direct = entity.Value;

        // Assert
        expression.Should().Be(direct);
    }
}
