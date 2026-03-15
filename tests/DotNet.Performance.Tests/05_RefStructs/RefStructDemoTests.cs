using DotNet.Performance.Examples.RefStructs;
using FluentAssertions;

namespace DotNet.Performance.Tests.RefStructs;

public sealed class RefStructDemoTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(64)]
    public void CreateStackBuffer_ValidSize_ReturnsBufferWithCorrectLength(int size)
    {
        // Arrange — heap-backed span for variable-size parameterised tests
        Span<int> data = new int[size];

        // Act
        StackBuffer buffer = RefStructDemo.CreateStackBuffer(data);

        // Assert
        buffer.Data.Length.Should().Be(size);
    }

    [Fact]
    public void CreateStackBuffer_EmptySpan_ThrowsArgumentOutOfRangeException()
    {
        // Act — Span<int>.Empty cannot be captured as a ref local in a lambda; pass inline
        Action act = () => RefStructDemo.CreateStackBuffer(Span<int>.Empty);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CreateStackBuffer_SpanExceedsMax_ThrowsArgumentOutOfRangeException()
    {
        // Act — create the span inline to avoid capturing a ref local
        Action act = () => RefStructDemo.CreateStackBuffer(new int[65]);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SumBuffer_ZeroInitializedBuffer_ReturnsZero()
    {
        // Arrange — stackalloc zero-initialises in C# by default
        Span<int> data = stackalloc int[5];
        StackBuffer buffer = RefStructDemo.CreateStackBuffer(data);

        // Act
        int result = RefStructDemo.SumBuffer(buffer);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void SumBuffer_PopulatedBuffer_ReturnsCorrectSum()
    {
        // Arrange
        Span<int> data = stackalloc int[4];
        data[0] = 1;
        data[1] = 2;
        data[2] = 3;
        data[3] = 4;
        StackBuffer buffer = RefStructDemo.CreateStackBuffer(data);

        // Act
        int result = RefStructDemo.SumBuffer(buffer);

        // Assert
        result.Should().Be(10);
    }
}
