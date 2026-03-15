using DotNet.Performance.Examples.Patterns;
using FluentAssertions;

namespace DotNet.Performance.Tests.Patterns;

public sealed class StructLayoutDemoTests
{
    [Fact]
    public void GetSequentialSize_ReturnsPositiveValue()
    {
        // Act
        int size = StructLayoutDemo.GetSequentialSize();

        // Assert
        size.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetExplicitSize_ReturnsPositiveValue()
    {
        // Act
        int size = StructLayoutDemo.GetExplicitSize();

        // Assert
        size.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetSequentialSize_IsAtLeastSumOfFieldSizes()
    {
        // The struct has two byte fields (1 byte each) and one int field (4 bytes).
        // With sequential layout the size is at minimum 6 bytes (before padding), typically 12.
        int minimumExpected = 1 + 4 + 1; // byte A + int B + byte C

        // Act
        int size = StructLayoutDemo.GetSequentialSize();

        // Assert
        size.Should().BeGreaterThanOrEqualTo(minimumExpected);
    }

    [Fact]
    public void GetExplicitSize_IsAtLeastEightBytes()
    {
        // ExplicitPoint: byte A at 0, byte C at 1, int B at 4 → spans bytes 0–7 → min 8 bytes
        // Act
        int size = StructLayoutDemo.GetExplicitSize();

        // Assert
        size.Should().BeGreaterThanOrEqualTo(8);
    }

    [Fact]
    public void ExplicitSize_IsSmallerThanOrEqualToSequentialSize()
    {
        // Explicit layout is intended to be no larger than sequential; this codifies the design intent.
        int sequential = StructLayoutDemo.GetSequentialSize();
        int explicitLayout = StructLayoutDemo.GetExplicitSize();

        // Assert
        explicitLayout.Should().BeLessThanOrEqualTo(sequential);
    }
}
