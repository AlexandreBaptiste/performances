using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.BoxingUnboxing;

namespace DotNet.Performance.Benchmarks.BoxingUnboxing;

/// <summary>
/// Compares boxing via non-generic <see cref="System.Collections.ArrayList"/> and interface
/// dispatch on structs against their boxing-free generic counterparts.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class BoxingBenchmark
{
    private readonly BoxingDemo _boxingDemo = new();
    private readonly InterfaceBoxingDemo _interfaceBoxingDemo = new();

    /// <summary>Gets or sets the number of elements to process.</summary>
    [Params(100, 1000)]
    public int Count { get; set; }

    /// <summary>
    /// Naive: each <see cref="int"/> is boxed into <see cref="object"/> via <c>ArrayList</c>.
    /// </summary>
    [Benchmark]
    public int ArrayListBoxing_Naive() => _boxingDemo.Naive(Count);

    /// <summary>
    /// Optimized: <c>List&lt;int&gt;</c> stores integers without boxing.
    /// </summary>
    [Benchmark]
    public int GenericList_Optimized() => _boxingDemo.Optimized(Count);

    /// <summary>
    /// Naive: each struct is boxed to <c>IValue</c> on every loop iteration.
    /// </summary>
    [Benchmark]
    public int InterfaceBoxing_Naive() => _interfaceBoxingDemo.Naive(Count);

    /// <summary>
    /// Optimized: generic constraint lets the JIT specialise the call, eliminating boxing.
    /// </summary>
    [Benchmark]
    public int GenericConstraint_Optimized() => _interfaceBoxingDemo.Optimized(Count);
}
