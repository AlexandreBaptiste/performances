using DotNet.Performance.Examples.StringOptimization;
using FluentAssertions;

namespace DotNet.Performance.Tests.StringOptimization;

public sealed class StringInternDemoTests
{
    [Fact]
    public void InternString_NullValue_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringInternDemo.InternString(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsInterned_NullValue_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => StringInternDemo.IsInterned(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("world")]
    [InlineData("")]
    public void InternString_ReturnsEqualString(string value)
    {
        // Act
        string interned = StringInternDemo.InternString(value);

        // Assert
        interned.Should().Be(value);
    }

    [Fact]
    public void InternString_SameValueReturnsSameReference()
    {
        // Arrange
        // Use a runtime-constructed string to avoid compile-time interning
        string a = new string(new char[] { 't', 'e', 's', 't' });
        string b = new string(new char[] { 't', 'e', 's', 't' });

        // Act
        string ia = StringInternDemo.InternString(a);
        string ib = StringInternDemo.InternString(b);

        // Assert
        object.ReferenceEquals(ia, ib).Should().BeTrue();
    }

    [Fact]
    public void IsInterned_AfterInterning_ReturnsTrue()
    {
        // Arrange
        string value = new string(new char[] { 'p', 'o', 'o', 'l', 'e', 'd' });
        StringInternDemo.InternString(value);

        // Act
        bool result = StringInternDemo.IsInterned(value);

        // Assert
        result.Should().BeTrue();
    }
}
