using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.SIMD;

namespace DotNet.Performance.Benchmarks.SIMD;

/// <summary>
/// Compares scalar array summation against SIMD-accelerated summation via <see cref="VectorTDemo"/>
/// at two representative input sizes to illustrate the throughput benefit of <c>Vector&lt;int&gt;</c>.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SIMDBenchmark
{
    /// <summary>Number of elements in the input array.</summary>
    [Params(1000, 10000)]
    public int Size { get; set; }

    private int[] _data = [];

    /// <summary>Initialises the input array once per parameter combination.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _data = new int[Size];
        for (int i = 0; i < Size; i++)
        {
            _data[i] = i;
        }
    }

    /// <summary>
    /// Scalar: sums <see cref="Size"/> integers with a plain <c>for</c> loop.
    /// </summary>
    [Benchmark(Baseline = true)]
    public long ScalarSum() => VectorTDemo.ScalarSum(_data);

    /// <summary>
    /// Vectorized: sums <see cref="Size"/> integers using <see cref="System.Numerics.Vector{T}"/> SIMD.
    /// </summary>
    [Benchmark]
    public long VectorizedSum() => VectorTDemo.VectorizedSum(_data);
}
