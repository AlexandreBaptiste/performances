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
    private readonly StackallocDemo _stackallocDemo = new();

    private const string SampleInput = "The quick brown fox jumps over the lazy dog";

    // Slice "quick" (index 4, length 5)
    private const int Start = 4;
    private const int Length = 5;

    /// <summary>Gets or sets the buffer size for stackalloc benchmarks.</summary>
    [Params(64, 256)]
    public int Size { get; set; }

    /// <summary>
    /// Naive: uses <see cref="string.Substring(int,int)"/> — allocates an intermediate string.
    /// </summary>
    [Benchmark(Baseline = true)]
    public string SubstringNaive() => _spanDemo.Naive(SampleInput, Start, Length);

    /// <summary>
    /// Optimized: uses <c>AsSpan().Slice()</c> — only one allocation at the final <c>new string(span)</c>.
    /// </summary>
    [Benchmark]
    public string SubstringOptimized() => _spanDemo.Optimized(SampleInput, Start, Length);

    /// <summary>
    /// Naive: allocates a new <see cref="byte"/> array on every call.
    /// </summary>
    [Benchmark]
    public long StackallocNaive() => _stackallocDemo.Naive(Size);

    /// <summary>
    /// Optimized: uses <c>stackalloc</c> to avoid heap allocation.
    /// </summary>
    [Benchmark]
    public long StackallocOptimized() => _stackallocDemo.Optimized(Size);
}
