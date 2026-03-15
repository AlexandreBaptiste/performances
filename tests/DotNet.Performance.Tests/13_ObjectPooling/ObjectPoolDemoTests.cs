using DotNet.Performance.Examples.ObjectPooling;
using FluentAssertions;

namespace DotNet.Performance.Tests.ObjectPooling;

public sealed class ObjectPoolDemoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void ProcessWithNew_ReturnsPositiveOrZeroCharCount(int count)
    {
        // Act
        int result = ObjectPoolDemo.ProcessWithNew(count);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void ProcessWithPool_ReturnsPositiveOrZeroCharCount(int count)
    {
        // Act
        int result = ObjectPoolDemo.ProcessWithPool(count);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public void ProcessWithPool_MatchesProcessWithNew(int count)
    {
        // Arrange
        int expected = ObjectPoolDemo.ProcessWithNew(count);

        // Act
        int result = ObjectPoolDemo.ProcessWithPool(count);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ProcessWithNew_ZeroCount_ReturnsZero()
    {
        // Act
        int result = ObjectPoolDemo.ProcessWithNew(0);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ProcessWithPool_ZeroCount_ReturnsZero()
    {
        // Act
        int result = ObjectPoolDemo.ProcessWithPool(0);

        // Assert
        result.Should().Be(0);
    }
}

public sealed class SimpleObjectPoolTests
{
    [Fact]
    public void Constructor_NullFactory_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new SimpleObjectPool<string>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Rent_WhenPoolEmpty_InvokesFactory()
    {
        // Arrange
        int factoryCallCount = 0;
        SimpleObjectPool<object> pool = new SimpleObjectPool<object>(() =>
        {
            factoryCallCount++;
            return new object();
        });

        // Act
        object item = pool.Rent();

        // Assert
        factoryCallCount.Should().Be(1);
        item.Should().NotBeNull();
    }

    [Fact]
    public void Return_ThenRent_ReusesObject()
    {
        // Arrange
        object original = new object();
        int factoryCallCount = 0;
        SimpleObjectPool<object> pool = new SimpleObjectPool<object>(() =>
        {
            factoryCallCount++;
            return original;
        });

        object rented = pool.Rent();
        pool.Return(rented);

        // Act
        object reused = pool.Rent();

        // Assert — factory should only have been called once; second Rent reuses from bag
        factoryCallCount.Should().Be(1);
        object.ReferenceEquals(rented, reused).Should().BeTrue();
    }
}
