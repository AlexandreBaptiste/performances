using DotNet.Performance.Examples.SIMD;
using FluentAssertions;

namespace DotNet.Performance.Tests.SIMD;

public sealed class VectorTDemoTests
{
    [Fact]
    public void ScalarSum_EmptyArray_ReturnsZero()
    {
        // Arrange
        int[] data = [];

        // Act
        long result = VectorTDemo.ScalarSum(data);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ScalarSum_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        int[] data = null!;

        // Act
        Action act = () => VectorTDemo.ScalarSum(data);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, 10L)]
    [InlineData(new[] { 10, 20, 30 }, 60L)]
    [InlineData(new[] { -5, 5 }, 0L)]
    public void ScalarSum_ReturnsCorrectSum(int[] data, long expected)
    {
        // Act
        long result = VectorTDemo.ScalarSum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void VectorizedSum_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        int[] data = null!;

        // Act
        Action act = () => VectorTDemo.VectorizedSum(data);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(100)]
    public void VectorizedSum_MatchesScalarSum(int length)
    {
        // Arrange
        int[] data = Enumerable.Range(1, length).ToArray();
        long expected = VectorTDemo.ScalarSum(data);

        // Act
        long result = VectorTDemo.VectorizedSum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void VectorizedSum_LargeArray_MatchesScalarSum()
    {
        // Arrange
        int[] data = Enumerable.Range(1, 10_000).ToArray();
        long expected = VectorTDemo.ScalarSum(data);

        // Act
        long result = VectorTDemo.VectorizedSum(data);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsHardwareAccelerated_ReturnsBool()
    {
        // Act
        bool accelerated = VectorTDemo.IsHardwareAccelerated;

        // Assert — verify it returns a deterministic boolean (true on modern hardware)
        accelerated.Should().Be(VectorTDemo.IsHardwareAccelerated);
    }
}
