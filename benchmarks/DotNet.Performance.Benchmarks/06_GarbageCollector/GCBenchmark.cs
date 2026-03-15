using BenchmarkDotNet.Attributes;
using System.Buffers;

namespace DotNet.Performance.Benchmarks.GarbageCollector;

/// <summary>
/// Compares repeatedly allocating new short-lived <see cref="byte"/> arrays (high GC pressure)
/// against renting from <see cref="ArrayPool{T}.Shared"/> (minimal GC pressure).
/// </summary>
/// <remarks>
/// Watch the Gen0/GC collections column: pooled allocations drive it to zero while
/// the naive pattern generates continuous Gen0 collections.
/// </remarks>
[MemoryDiagnoser]
[RankColumn]
public class GCBenchmark
{
    /// <summary>Gets or sets the size of each buffer in bytes.</summary>
    [Params(1_000, 10_000)]
    public int Size { get; set; }

    /// <summary>
    /// Naive: allocates a fresh <see cref="byte"/> array on every iteration.
    /// Each array is a new heap object, causing Gen0 GC pressure.
    /// </summary>
    [Benchmark]
    public long AllocateShortLived_Naive()
    {
        byte[] buffer = new byte[Size];

        long sum = 0;

        for (int i = 0; i < Size; i++)
        {
            buffer[i] = (byte)(i % 256);
            sum += buffer[i];
        }

        return sum;
    }

    /// <summary>
    /// Optimized: rents a buffer from <see cref="ArrayPool{T}.Shared"/> and returns it after use.
    /// No new heap allocation after the pool is warmed up.
    /// </summary>
    [Benchmark]
    public long AllocateShortLived_Pooled()
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(Size);

        try
        {
            long sum = 0;

            for (int i = 0; i < Size; i++)
            {
                buffer[i] = (byte)(i % 256);
                sum += buffer[i];
            }

            return sum;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
