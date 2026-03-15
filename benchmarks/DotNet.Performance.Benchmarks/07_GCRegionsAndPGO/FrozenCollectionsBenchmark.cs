using BenchmarkDotNet.Attributes;
using System.Collections.Frozen;
using DotNet.Performance.Examples.GCRegionsAndPGO;

namespace DotNet.Performance.Benchmarks.GCRegionsAndPGO;

/// <summary>
/// Compares key lookups in a mutable <see cref="Dictionary{TKey,TValue}"/> (Naive) against
/// a <see cref="FrozenDictionary{TKey,TValue}"/> (Optimized) to show the read-path speedup.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class FrozenCollectionsBenchmark
{
    private Dictionary<string, int> _mutableDict = null!;
    private FrozenDictionary<string, int> _frozenDict = null!;

    private static readonly string[] Keys = ["alpha", "beta", "gamma", "delta", "epsilon"];

    /// <summary>Initialises dictionaries once before all benchmark iterations.</summary>
    [GlobalSetup]
    public void Setup()
    {
        IEnumerable<KeyValuePair<string, int>> pairs = Keys.Select((k, i) => new KeyValuePair<string, int>(k, i));
        _mutableDict = new Dictionary<string, int>(pairs);
        _frozenDict  = FrozenCollectionsDemo.CreateFrozenDictionary(pairs);
    }

    /// <summary>
    /// Naive: performs lookups on a mutable <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    [Benchmark]
    public int Naive()
    {
        int total = 0;

        for (int i = 0; i < Keys.Length; i++)
        {
            total += FrozenCollectionsDemo.LookupNaive(_mutableDict, Keys[i]);
        }

        return total;
    }

    /// <summary>
    /// Optimized: performs lookups on a <see cref="FrozenDictionary{TKey,TValue}"/>
    /// which uses a specialised hash strategy for faster reads.
    /// </summary>
    [Benchmark]
    public int Optimized()
    {
        int total = 0;

        for (int i = 0; i < Keys.Length; i++)
        {
            total += FrozenCollectionsDemo.LookupFrozen(_frozenDict, Keys[i]);
        }

        return total;
    }
}
