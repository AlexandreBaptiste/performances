using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.ObjectPooling;

namespace DotNet.Performance.Benchmarks.ObjectPooling;

/// <summary>
/// Compares <see cref="ObjectPoolDemo.ProcessWithNew"/> (allocates a new <c>StringBuilder</c> per
/// iteration) against <see cref="ObjectPoolDemo.ProcessWithPool"/> (rents and returns from a
/// <see cref="SimpleObjectPool{T}"/>) to quantify the GC-pressure benefit of object pooling.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class ObjectPoolBenchmark
{
    private const int Count = 1000;

    /// <summary>
    /// Naive: creates a fresh <see cref="System.Text.StringBuilder"/> for each of the
    /// <see cref="Count"/> iterations, leaving all instances for the GC to reclaim.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int ProcessWithNew() => ObjectPoolDemo.ProcessWithNew(Count);

    /// <summary>
    /// Optimized: rents one (or a small number of) pooled <see cref="System.Text.StringBuilder"/>
    /// instance(s), reusing them across all iterations to eliminate repeated heap allocations.
    /// </summary>
    [Benchmark]
    public int ProcessWithPool() => ObjectPoolDemo.ProcessWithPool(Count);
}
