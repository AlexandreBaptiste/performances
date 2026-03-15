using DotNet.Performance.Examples.Reflection;
using FluentAssertions;

namespace DotNet.Performance.Tests.Reflection;

public sealed class ActivatorVsNewDemoTests
{
    [Fact]
    public void CreateViaActivator_ReturnsNotNullInstance()
    {
        // Act
        SampleEntity entity = ActivatorVsNewDemo.CreateViaActivator();

        // Assert
        entity.Should().NotBeNull();
    }

    [Fact]
    public void CreateViaActivator_ReturnsDefaultPropertyValues()
    {
        // Act
        SampleEntity entity = ActivatorVsNewDemo.CreateViaActivator();

        // Assert
        entity.Value.Should().Be(0);
        entity.Name.Should().Be("");
    }

    [Fact]
    public void CreateViaNegativeNew_ReturnsNotNullSampleEntityInstance()
    {
        // Act
        SampleEntity entity = ActivatorVsNewDemo.CreateViaNegativeNew<SampleEntity>();

        // Assert
        entity.Should().NotBeNull();
    }

    [Fact]
    public void CreateViaNegativeNew_ReturnsDefaultPropertyValues()
    {
        // Act
        SampleEntity entity = ActivatorVsNewDemo.CreateViaNegativeNew<SampleEntity>();

        // Assert
        entity.Value.Should().Be(0);
        entity.Name.Should().Be("");
    }

    [Fact]
    public void CreateViaActivator_ReturnsDifferentInstancesOnEachCall()
    {
        // Act
        SampleEntity first = ActivatorVsNewDemo.CreateViaActivator();
        SampleEntity second = ActivatorVsNewDemo.CreateViaActivator();

        // Assert
        first.Should().NotBeSameAs(second);
    }

    [Fact]
    public void CreateViaNegativeNew_ReturnsDifferentInstancesOnEachCall()
    {
        // Act
        SampleEntity first = ActivatorVsNewDemo.CreateViaNegativeNew<SampleEntity>();
        SampleEntity second = ActivatorVsNewDemo.CreateViaNegativeNew<SampleEntity>();

        // Assert
        first.Should().NotBeSameAs(second);
    }
}
