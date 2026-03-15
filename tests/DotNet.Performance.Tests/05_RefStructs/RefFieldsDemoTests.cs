using DotNet.Performance.Examples.RefStructs;
using FluentAssertions;

namespace DotNet.Performance.Tests.RefStructs;

public sealed class RefFieldsDemoTests
{
    [Fact]
    public void CreateRefCell_ReturnsCell_WithSameValueAsOriginal()
    {
        // Arrange
        int value = 10;

        // Act
        RefCell<int> cell = RefFieldsDemo.CreateRefCell(ref value);

        // Assert
        cell.Value.Should().Be(10);
    }

    [Fact]
    public void Increment_IncreasesOriginalVariable_ByOne()
    {
        // Arrange
        int value = 5;
        RefCell<int> cell = RefFieldsDemo.CreateRefCell(ref value);

        // Act
        RefFieldsDemo.Increment(ref cell);

        // Assert
        value.Should().Be(6);
    }

    [Fact]
    public void Increment_MultipleTimes_ReflectsAllChangesOnOriginalVariable()
    {
        // Arrange
        int value = 0;
        RefCell<int> cell = RefFieldsDemo.CreateRefCell(ref value);

        // Act
        RefFieldsDemo.Increment(ref cell);
        RefFieldsDemo.Increment(ref cell);
        RefFieldsDemo.Increment(ref cell);

        // Assert
        value.Should().Be(3);
    }

    [Fact]
    public void CellValue_ReflectsDirectMutationOnOriginalVariable()
    {
        // Arrange
        int value = 0;
        RefCell<int> cell = RefFieldsDemo.CreateRefCell(ref value);

        // Act — mutate the original, not through the cell
        value = 99;

        // Assert — the cell sees the updated value via the ref field
        cell.Value.Should().Be(99);
    }
}
