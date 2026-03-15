using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.MemoryModel;

namespace DotNet.Performance.Benchmarks.MemoryModel;

/// <summary>
/// Compares the cost of allocating a new <see cref="int"/> array on every call (Naive)
/// against reusing a pre-allocated instance buffer (Optimized).
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class AllocationPressureBenchmark
{
    private readonly AllocationPressure _example = new();

    /// <summary>Gets or sets the number of elements to process.</summary>
    [Params(100, 1000)]
    public int Size { get; set; }

    /// <summary>
    /// Allocates a new <see cref="int"/> array on each invocation.
    /// </summary>
    [Benchmark]
    public long Naive() => _example.Naive(Size);

    /// <summary>
    /// Reuses a pre-allocated instance buffer — no per-call heap allocation.
    /// </summary>
    [Benchmark]
    public long Optimized() => _example.Optimized(Size);
}
