// ============================================================
// Concept  : NativeMemory — unmanaged heap allocation
// Summary  : Allocates and frees native memory outside the GC heap for interop and high-performance scenarios
// When to use   : Large buffers shared with native code, or avoiding GC pressure for very large short-lived arrays
// When NOT to use: General-purpose buffers reachable only from managed code — prefer ArrayPool<T> or stackalloc
// ============================================================

// UNSAFE — educational only. Always free native memory.
using System.Runtime.InteropServices;

namespace DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;

/// <summary>
/// Demonstrates allocating and releasing native (unmanaged) memory via <see cref="NativeMemory"/>.
/// </summary>
/// <remarks>
/// <para>
/// Native memory lives outside the GC heap and is invisible to the garbage collector.
/// It must always be freed explicitly; wrapping allocations in a <c>try/finally</c> block
/// guarantees deallocation even when an exception is thrown.
/// </para>
/// <para>
/// Use <see cref="NativeMemory.AlignedAlloc"/> when the target hardware or SIMD intrinsics require
/// a specific memory alignment (e.g., 16-byte or 64-byte alignment for AVX512 loads).
/// </para>
/// </remarks>
public static class NativeMemoryDemo
{
    /// <summary>
    /// Allocates native memory for <paramref name="count"/> integers via <c>NativeMemory.Alloc</c>,
    /// fills the region with values 0, 1, …, count-1, computes the sum, then frees the memory
    /// via <c>NativeMemory.Free</c>.
    /// </summary>
    /// <param name="count">Number of integers to allocate; must be greater than zero.</param>
    /// <returns>The sum of the integer sequence 0, 1, …, <paramref name="count"/>-1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is less than or equal to zero.
    /// </exception>
    public static unsafe long AllocateAndSum(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int* ptr = (int*)NativeMemory.Alloc((nuint)(count * sizeof(int)));

        try
        {
            for (int i = 0; i < count; i++)
            {
                ptr[i] = i;
            }

            long sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += ptr[i];
            }

            return sum;
        }
        finally
        {
            // Always free — NativeMemory is not tracked by the GC; missing this causes a memory leak.
            NativeMemory.Free(ptr);
        }
    }

    /// <summary>
    /// Allocates aligned native memory for <paramref name="count"/> integers,
    /// zero-fills the region, then frees it — demonstrating the aligned allocation lifecycle.
    /// </summary>
    /// <param name="count">Number of integers to allocate; must be greater than zero.</param>
    /// <param name="alignment">
    /// Required byte alignment; must be a power of two and at least the size of a pointer.
    /// </param>
    /// <returns>The number of integers allocated (always equals <paramref name="count"/>).</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is less than or equal to zero.
    /// </exception>
    public static int AllocateAligned(int count, nuint alignment)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        return AllocateAlignedCore(count, alignment);
    }

    private static unsafe int AllocateAlignedCore(int count, nuint alignment)
    {
        void* ptr = NativeMemory.AlignedAlloc((nuint)(count * sizeof(int)), alignment);

        try
        {
            // Zero the region to demonstrate safe initialisation of aligned native memory.
            NativeMemory.Clear(ptr, (nuint)(count * sizeof(int)));

            return count;
        }
        finally
        {
            // AlignedAlloc memory must be freed with AlignedFree — not NativeMemory.Free.
            NativeMemory.AlignedFree(ptr);
        }
    }
}
