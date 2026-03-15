using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.Inlining;

namespace DotNet.Performance.Benchmarks.Inlining;

/// <summary>
/// Compares <see cref="AggressiveInliningDemo.Naive"/> (no inlining hint) against
/// <see cref="AggressiveInliningDemo.Optimized"/> (with <c>AggressiveInlining</c>)
/// for a tight arithmetic computation.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class InliningBenchmark
{
    private const int InputValue = 42;

    /// <summary>
    /// Naive: computes <c>x² + 2x + 1</c> without an inlining hint.
    /// The JIT may or may not inline the call.
    /// </summary>
    [Benchmark]
    public int Naive() => AggressiveInliningDemo.Naive(InputValue);

    /// <summary>
    /// Optimized: same computation with <see cref="System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining"/>,
    /// guaranteeing the call site is inlined by the JIT.
    /// </summary>
    [Benchmark]
    public int Optimized() => AggressiveInliningDemo.Optimized(InputValue);
}
