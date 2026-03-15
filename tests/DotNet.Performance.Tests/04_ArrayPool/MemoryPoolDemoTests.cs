using DotNet.Performance.Examples.ArrayPool;
using FluentAssertions;

namespace DotNet.Performance.Tests.ArrayPool;

public sealed class MemoryPoolDemoTests
{
    private readonly MemoryPoolDemo _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(256)]
    public void ProcessWithNew_ReturnsCorrectChecksum(int size)
    {
        // Arrange
        byte expected = ComputeExpectedChecksum(size);

        // Act
        byte result = _sut.ProcessWithNew(size);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(256)]
    public void ProcessWithMemoryPool_ReturnsChecksumMatchingProcessWithNew(int size)
    {
        // Arrange
        byte expected = _sut.ProcessWithNew(size);

        // Act
        byte result = _sut.ProcessWithMemoryPool(size);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ProcessWithNew_ThrowsArgumentOutOfRangeException_WhenSizeIsZero()
    {
        // Act
        Action act = () => _sut.ProcessWithNew(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ProcessWithMemoryPool_ThrowsArgumentOutOfRangeException_WhenSizeIsZero()
    {
        // Act
        Action act = () => _sut.ProcessWithMemoryPool(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static byte ComputeExpectedChecksum(int size)
    {
        byte checksum = 0;

        for (int i = 0; i < size; i++)
        {
            checksum ^= (byte)(i % 256);
        }

        return checksum;
    }
}
