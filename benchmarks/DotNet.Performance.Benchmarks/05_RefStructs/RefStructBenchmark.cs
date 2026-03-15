using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.RefStructs;

namespace DotNet.Performance.Benchmarks.RefStructs;

/// <summary>
/// Compares summing integers via a <c>ref struct</c> + <c>stackalloc</c> buffer (zero heap allocation)
/// against allocating a plain <see cref="int"/> array on the heap each time.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class RefStructBenchmark
{
    /// <summary>Gets or sets the number of integers to sum in each benchmark iteration.</summary>
    [Params(16, 64)]
    public int Size { get; set; }

    /// <summary>
    /// Naive: allocates a new <see cref="int"/> array on every call.
    /// </summary>
    [Benchmark]
    public int Naive()
    {
        int[] array = new int[Size];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }

        int sum = 0;

        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }

        return sum;
    }

    /// <summary>
    /// Optimized: uses a <c>ref struct StackBuffer</c> backed by <c>stackalloc</c> — zero heap allocation.
    /// The <c>stackalloc</c> is in the benchmark method itself (the caller), which is the required C# pattern.
    /// </summary>
    [Benchmark]
    public int Optimized()
    {
        // stackalloc must live in the method that uses the buffer — it cannot be returned from a factory.
        Span<int> scratch = stackalloc int[Size];

        for (int i = 0; i < scratch.Length; i++)
        {
            scratch[i] = i;
        }

        StackBuffer buffer = RefStructDemo.CreateStackBuffer(scratch);
        return RefStructDemo.SumBuffer(buffer);
    }
}
