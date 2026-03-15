using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;

namespace DotNet.Performance.Benchmarks.SkipLocalsInitAndUnsafe;

/// <summary>
/// Compares a naive <c>stackalloc</c> path that performs a redundant <c>Clear()</c> against
/// an optimised path that relies on <c>[module: SkipLocalsInit]</c> to skip zero-initialisation.
/// Both produce the same result; the optimised version avoids the unnecessary memory write pass.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SkipLocalsInitBenchmark
{
    /// <summary>Gets or sets the number of bytes to fill and sum in each benchmark iteration.</summary>
    [Params(64, 256)]
    public int Size { get; set; }

    /// <summary>
    /// Naive path: performs an explicit <c>buffer.Clear()</c> before filling — redundant work.
    /// </summary>
    [Benchmark]
    public int Naive() => SkipLocalsInitDemo.Naive(Size);

    /// <summary>
    /// Optimised path: relies on <c>[module: SkipLocalsInit]</c>; skips zero-initialisation
    /// because every byte is written before it is read.
    /// </summary>
    [Benchmark]
    public int Optimized() => SkipLocalsInitDemo.Optimized(Size);
}
