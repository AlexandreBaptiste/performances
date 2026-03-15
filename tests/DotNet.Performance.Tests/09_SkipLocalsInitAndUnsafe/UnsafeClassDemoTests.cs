using DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;
using FluentAssertions;

namespace DotNet.Performance.Tests.SkipLocalsInitAndUnsafe;

public sealed class UnsafeClassDemoTests
{
    [Fact]
    public void CopyWithOffset_ValidOffset_CopiesCorrectElements()
    {
        // Arrange
        int[] source = [10, 20, 30, 40, 50];
        int[] destination = new int[3];

        // Act
        int count = UnsafeClassDemo.CopyWithOffset(source, destination, 1);

        // Assert
        count.Should().Be(3);
        destination.Should().Equal(20, 30, 40);
    }

    [Fact]
    public void CopyWithOffset_OffsetAtLastElement_CopiesSingleElement()
    {
        // Arrange
        int[] source = [10, 20, 30];
        int[] destination = new int[5];

        // Act
        int count = UnsafeClassDemo.CopyWithOffset(source, destination, 2);

        // Assert
        count.Should().Be(1);
        destination[0].Should().Be(30);
    }

    [Fact]
    public void CopyWithOffset_DestinationSmallerThanRemainingSource_ClampsToDestinationLength()
    {
        // Arrange
        int[] source = [1, 2, 3, 4, 5];
        int[] destination = new int[2];

        // Act
        int count = UnsafeClassDemo.CopyWithOffset(source, destination, 0);

        // Assert
        count.Should().Be(2);
        destination.Should().Equal(1, 2);
    }

    [Fact]
    public void CopyWithOffset_NullSource_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => UnsafeClassDemo.CopyWithOffset(null!, new int[1], 0);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CopyWithOffset_NullDestination_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => UnsafeClassDemo.CopyWithOffset(new int[1], null!, 0);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CopyWithOffset_NegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        // Act
        Action act = () => UnsafeClassDemo.CopyWithOffset(new int[3], new int[3], -1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CopyWithOffset_OffsetEqualToSourceLength_ThrowsArgumentOutOfRangeException()
    {
        // Arrange — offset == length means no valid start index
        int[] source = [1, 2, 3];

        // Act
        Action act = () => UnsafeClassDemo.CopyWithOffset(source, new int[3], source.Length);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetByteSize_Int_ReturnsFour()
    {
        // Act
        int size = UnsafeClassDemo.GetByteSize<int>();

        // Assert
        size.Should().Be(4);
    }

    [Fact]
    public void GetByteSize_Long_ReturnsEight()
    {
        // Act
        int size = UnsafeClassDemo.GetByteSize<long>();

        // Assert
        size.Should().Be(8);
    }
}
