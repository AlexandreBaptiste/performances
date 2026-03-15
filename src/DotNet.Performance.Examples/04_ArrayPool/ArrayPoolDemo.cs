// ============================================================
// Concept  : ArrayPool<T> and MemoryPool<T>
// Summary  : Demonstrates renting buffers from ArrayPool<T> to avoid repeated heap allocations
// When to use   : High-frequency methods that need temporary byte/char arrays of varying sizes
// When NOT to use: Very small or infrequent allocations where pool bookkeeping exceeds the benefit
// ============================================================

using System.Buffers;

namespace DotNet.Performance.Examples.ArrayPool;

/// <summary>
/// Compares allocating a fresh <see cref="byte"/> array on every call (high GC pressure) against
/// renting from <see cref="ArrayPool{T}.Shared"/> and returning the buffer when done (minimal GC).
/// </summary>
/// <example>
/// <code>
/// var demo = new ArrayPoolDemo();
/// long sumNaive     = demo.Naive(1000);     // allocates new byte[1000] each time
/// long sumOptimized = demo.Optimized(1000); // rents from and returns to ArrayPool
/// </code>
/// </example>
public sealed class ArrayPoolDemo
{
    /// <summary>
    /// Allocates a new <see cref="byte"/> array on every invocation, fills it with
    /// values <c>i % 256</c>, and returns their sum.
    /// This pattern allocates one array per call and adds GC pressure over time.
    /// </summary>
    /// <param name="size">Number of bytes to allocate and process. Must be positive.</param>
    /// <returns>Sum of byte values 0 through <paramref name="size"/> − 1 (mod 256).</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public long Naive(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // New allocation every call — the GC must eventually reclaim this array.
        byte[] buffer = new byte[size];

        for (int i = 0; i < size; i++)
        {
            buffer[i] = (byte)(i % 256);
        }

        long sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += buffer[i];
        }

        return sum;
    }

    /// <summary>
    /// Rents a buffer from <see cref="ArrayPool{T}.Shared"/>, fills it, computes the sum,
    /// then returns the buffer in a <c>finally</c> block to guarantee release even on exceptions.
    /// Note: rented arrays may be larger than requested — only the first <paramref name="size"/>
    /// elements are used.
    /// </summary>
    /// <param name="size">Number of bytes to process. Must be positive.</param>
    /// <returns>Sum of byte values 0 through <paramref name="size"/> − 1 (mod 256).</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public long Optimized(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // Rent a buffer — the pool recycles previously returned arrays to avoid re-allocation.
        byte[] buffer = ArrayPool<byte>.Shared.Rent(size);

        try
        {
            // Use exactly [0, size) elements; the rented array may be larger.
            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)(i % 256);
            }

            long sum = 0;

            for (int i = 0; i < size; i++)
            {
                sum += buffer[i];
            }

            return sum;
        }
        finally
        {
            // Always return the buffer so it can be reused — even if an exception is thrown.
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
