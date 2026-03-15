using DotNet.Performance.Examples.SIMD;
using FluentAssertions;

namespace DotNet.Performance.Tests.SIMD;

public sealed class Vector128DemoTests
{
    [Fact]
    public void DotProductScalar_NullA_ThrowsArgumentNullException()
    {
        // Arrange
        float[] a = null!;
        float[] b = [1f, 2f];

        // Act
        Action act = () => Vector128Demo.DotProductScalar(a, b);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void DotProductScalar_NullB_ThrowsArgumentNullException()
    {
        // Arrange
        float[] a = [1f, 2f];
        float[] b = null!;

        // Act
        Action act = () => Vector128Demo.DotProductScalar(a, b);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new float[] { 1f, 2f, 3f, 4f }, new float[] { 4f, 3f, 2f, 1f }, 20f)]
    [InlineData(new float[] { 1f, 0f, 0f, 0f }, new float[] { 5f, 5f, 5f, 5f }, 5f)]
    [InlineData(new float[] { 2f, 2f }, new float[] { 3f, 3f }, 12f)]
    public void DotProductScalar_ReturnsCorrectResult(float[] a, float[] b, float expected)
    {
        // Act
        float result = Vector128Demo.DotProductScalar(a, b);

        // Assert
        result.Should().BeApproximately(expected, precision: 0.001f);
    }

    [Fact]
    public void DotProductVector128_NullA_ThrowsArgumentNullException()
    {
        // Arrange
        float[] a = null!;
        float[] b = [1f, 2f, 3f, 4f];

        // Act
        Action act = () => Vector128Demo.DotProductVector128(a, b);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void DotProductVector128_NullB_ThrowsArgumentNullException()
    {
        // Arrange
        float[] a = [1f, 2f, 3f, 4f];
        float[] b = null!;

        // Act
        Action act = () => Vector128Demo.DotProductVector128(a, b);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(100)]
    public void DotProductVector128_MatchesScalar(int length)
    {
        // Arrange
        float[] a = Enumerable.Range(1, length).Select(x => (float)x).ToArray();
        float[] b = Enumerable.Range(length, length).Select(x => (float)x).ToArray();
        float expected = Vector128Demo.DotProductScalar(a, b);

        // Act
        float result = Vector128Demo.DotProductVector128(a, b);

        // Assert
        result.Should().BeApproximately(expected, precision: 0.01f);
    }

    [Theory]
    [InlineData(new float[] { 1f, 2f, 3f, 4f }, new float[] { 4f, 3f, 2f, 1f }, 20f)]
    [InlineData(new float[] { 1f, 0f, 0f, 0f }, new float[] { 5f, 5f, 5f, 5f }, 5f)]
    public void DotProductVector128_ReturnsCorrectResult(float[] a, float[] b, float expected)
    {
        // Act
        float result = Vector128Demo.DotProductVector128(a, b);

        // Assert
        result.Should().BeApproximately(expected, precision: 0.001f);
    }
}
