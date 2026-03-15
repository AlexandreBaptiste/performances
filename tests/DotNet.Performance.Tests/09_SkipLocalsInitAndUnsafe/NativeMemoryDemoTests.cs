using DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;
using FluentAssertions;

namespace DotNet.Performance.Tests.SkipLocalsInitAndUnsafe;

public sealed class NativeMemoryDemoTests
{
    [Fact]
    public void AllocateAndSum_CountOne_ReturnsZero()
    {
        // Act
        long result = NativeMemoryDemo.AllocateAndSum(1);

        // Assert — sequence is [0]; sum = 0
        result.Should().Be(0L);
    }

    [Fact]
    public void AllocateAndSum_CountFive_ReturnsTen()
    {
        // Act
        long result = NativeMemoryDemo.AllocateAndSum(5);

        // Assert — 0+1+2+3+4 = 10
        result.Should().Be(10L);
    }

    [Fact]
    public void AllocateAndSum_CountTen_ReturnsFortyFive()
    {
        // Act
        long result = NativeMemoryDemo.AllocateAndSum(10);

        // Assert — 0+1+…+9 = 45
        result.Should().Be(45L);
    }

    [Fact]
    public void AllocateAndSum_ZeroCount_ThrowsArgumentOutOfRangeException()
    {
        // Act
        Action act = () => NativeMemoryDemo.AllocateAndSum(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AllocateAndSum_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        // Act
        Action act = () => NativeMemoryDemo.AllocateAndSum(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AllocateAligned_ValidCountAndAlignment_ReturnsCount()
    {
        // Act
        int result = NativeMemoryDemo.AllocateAligned(4, 16);

        // Assert
        result.Should().Be(4);
    }

    [Fact]
    public void AllocateAligned_LargeAlignmentAndCount_ReturnsCount()
    {
        // Act
        int result = NativeMemoryDemo.AllocateAligned(16, 64);

        // Assert
        result.Should().Be(16);
    }

    [Fact]
    public void AllocateAligned_ZeroCount_ThrowsArgumentOutOfRangeException()
    {
        // Act
        Action act = () => NativeMemoryDemo.AllocateAligned(0, 16);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AllocateAligned_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        // Act
        Action act = () => NativeMemoryDemo.AllocateAligned(-5, 16);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
