using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.Patterns;

namespace DotNet.Performance.Benchmarks.Patterns;

/// <summary>
/// Compares <see cref="CollectionsMarshalDemo.SumWithLinq"/> (LINQ + enumerator allocation) against
/// <see cref="CollectionsMarshalDemo.SumWithMarshal"/> (<see cref="System.Runtime.InteropServices.CollectionsMarshal.AsSpan{T}"/>
/// zero-allocation span loop) to illustrate the overhead reduction achievable by bypassing the
/// standard <see cref="List{T}"/> enumeration path.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class PatternsBenchmark
{
    private List<int> _data = [];

    /// <summary>Populates the input list once per benchmark run.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _data = Enumerable.Range(1, 10_000).ToList();
    }

    /// <summary>
    /// Naive: sums the list elements via LINQ's <c>Sum()</c>, which allocates an enumerator
    /// and incurs virtual-dispatch overhead per element.
    /// </summary>
    [Benchmark(Baseline = true)]
    public long SumWithLinq() => CollectionsMarshalDemo.SumWithLinq(_data);

    /// <summary>
    /// Optimized: sums the list elements via a direct <see cref="System.Span{T}"/> obtained
    /// from <see cref="System.Runtime.InteropServices.CollectionsMarshal.AsSpan{T}"/>,
    /// with zero allocations and no virtual dispatch.
    /// </summary>
    [Benchmark]
    public long SumWithMarshal() => CollectionsMarshalDemo.SumWithMarshal(_data);
}
