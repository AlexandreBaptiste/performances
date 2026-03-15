using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.SpanAndMemory;

namespace DotNet.Performance.Benchmarks.SpanAndMemory;

/// <summary>
/// Compares <see cref="string.Substring(int,int)"/> (allocates an intermediate string)
/// against <c>AsSpan().Slice()</c> (single final allocation only).
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SpanBenchmark
{
    private readonly SpanDemo _spanDemo = new();

    private const string SampleInput = "The quick brown fox jumps over the lazy dog";

    // Slice "quick" (index 4, length 5)
    private const int Start = 4;
    private const int Length = 5;

    /// <summary>
    /// Naive: uses <see cref="string.Substring(int,int)"/> — allocates an intermediate string.
    /// </summary>
    [Benchmark]
    public string Naive() => _spanDemo.Naive(SampleInput, Start, Length);

    /// <summary>
    /// Optimized: uses <c>AsSpan().Slice()</c> — only one allocation at the final <c>new string(span)</c>.
    /// </summary>
    [Benchmark]
    public string Optimized() => _spanDemo.Optimized(SampleInput, Start, Length);
}
