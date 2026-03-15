using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.ArrayPool;

namespace DotNet.Performance.Benchmarks.ArrayPool;

/// <summary>
/// Compares allocating a fresh <see cref="byte"/> array on every call (Naive) against
/// renting from <see cref="System.Buffers.ArrayPool{T}.Shared"/> (Optimized).
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class ArrayPoolBenchmark
{
    private readonly ArrayPoolDemo _arrayPoolDemo = new();
    private readonly MemoryPoolDemo _memoryPoolDemo = new();

    /// <summary>Gets or sets the buffer size to use in each benchmark iteration.</summary>
    [Params(100, 1000)]
    public int Size { get; set; }

    /// <summary>
    /// Naive: allocates a new <see cref="byte"/> array on every call.
    /// </summary>
    [Benchmark]
    public long Naive() => _arrayPoolDemo.Naive(Size);

    /// <summary>
    /// Optimized: rents from <see cref="System.Buffers.ArrayPool{T}.Shared"/> — no per-call allocation.
    /// </summary>
    [Benchmark]
    public long Optimized() => _arrayPoolDemo.Optimized(Size);
}
