using DotNet.Performance.Examples.Patterns;
using FluentAssertions;

namespace DotNet.Performance.Tests.Patterns;

public sealed class ValueTaskDemoTests
{
    [Fact]
    public async Task GetCachedValueAsync_ReturnsComputedValue()
    {
        // Arrange
        ValueTaskDemo demo = new ValueTaskDemo();

        // Act
        int result = await demo.GetCachedValueAsync(5);

        // Assert
        result.Should().Be(25); // 5 * 5
    }

    [Fact]
    public async Task GetCachedValueAsync_SecondCallReturnsCachedValue()
    {
        // Arrange
        ValueTaskDemo demo = new ValueTaskDemo();
        await demo.GetCachedValueAsync(4);

        // Act
        int result = await demo.GetCachedValueAsync(4);

        // Assert
        result.Should().Be(16); // 4 * 4, served from cache
    }

    [Fact]
    public async Task GetValueWithTaskAsync_ReturnsComputedValue()
    {
        // Arrange
        ValueTaskDemo demo = new ValueTaskDemo();

        // Act
        int result = await demo.GetValueWithTaskAsync(3);

        // Assert
        result.Should().Be(9); // 3 * 3
    }

    [Fact]
    public async Task GetValueWithTaskAsync_SecondCallReturnsCachedValue()
    {
        // Arrange
        ValueTaskDemo demo = new ValueTaskDemo();
        await demo.GetValueWithTaskAsync(6);

        // Act
        int result = await demo.GetValueWithTaskAsync(6);

        // Assert
        result.Should().Be(36); // 6 * 6
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(10)]
    public async Task GetCachedValueAsync_MatchesGetValueWithTaskAsync(int key)
    {
        // Arrange
        ValueTaskDemo demoForValueTask = new ValueTaskDemo();
        ValueTaskDemo demoForTask = new ValueTaskDemo();

        // Act
        int valueTaskResult = await demoForValueTask.GetCachedValueAsync(key);
        int taskResult = await demoForTask.GetValueWithTaskAsync(key);

        // Assert
        valueTaskResult.Should().Be(taskResult);
    }

    [Fact]
    public async Task GetCachedValueAsync_DifferentKeys_ReturnDifferentValues()
    {
        // Arrange
        ValueTaskDemo demo = new ValueTaskDemo();

        // Act
        int result2 = await demo.GetCachedValueAsync(2);
        int result3 = await demo.GetCachedValueAsync(3);

        // Assert
        result2.Should().Be(4);
        result3.Should().Be(9);
    }
}
