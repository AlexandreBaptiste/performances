using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.StringOptimization;

namespace DotNet.Performance.Benchmarks.StringOptimization;

/// <summary>
/// Compares <see cref="StringConcatDemo.Naive"/> (repeated <c>+=</c> string concatenation)
/// against <see cref="StringConcatDemo.Optimized"/> (<see cref="System.Text.StringBuilder"/>)
/// to demonstrate the allocation and throughput impact of O(n²) vs O(n) string building.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class StringBenchmark
{
    /// <summary>Number of <c>"Hello"</c> segments to concatenate.</summary>
    [Params(10, 100)]
    public int Count { get; set; }

    /// <summary>
    /// Naive: builds the string with repeated <c>+=</c>, allocating a new string per iteration.
    /// </summary>
    [Benchmark(Baseline = true)]
    public string Naive() => StringConcatDemo.Naive(Count);

    /// <summary>
    /// Optimized: builds the string once with <see cref="System.Text.StringBuilder"/>,
    /// allocating only the final result string.
    /// </summary>
    [Benchmark]
    public string Optimized() => StringConcatDemo.Optimized(Count);
}
