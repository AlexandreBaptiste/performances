// ============================================================
// Concept  : Memory Model
// Summary  : Demonstrates how repeated array allocations create GC pressure vs. buffer reuse
// When to use   : High-frequency methods that process varying-size data buffers
// When NOT to use: Single-call or low-frequency scenarios where simplicity matters more
// ============================================================

namespace DotNet.Performance.Examples.MemoryModel;

/// <summary>
/// Compares a pattern that allocates a new array on every call (high GC pressure)
/// against one that reuses a pre-allocated instance field buffer (zero per-call allocation).
/// </summary>
/// <example>
/// <code>
/// var demo = new AllocationPressure();
/// long sum = demo.Naive(100);      // allocates a new int[100] every call
/// long sum = demo.Optimized(100);  // reuses the internal int[1024] buffer
/// </code>
/// </example>
public sealed class AllocationPressure
{
    // Pre-allocated buffer reused across all Optimized() calls (supports up to 1 024 ints).
    private readonly int[] _buffer = new int[1024];

    /// <summary>
    /// Allocates a new <see cref="int"/> array on every invocation, fills it with
    /// sequential values, and returns their sum.
    /// This pattern creates GC pressure when called repeatedly.
    /// </summary>
    /// <param name="size">The number of elements to allocate and process. Must be positive.</param>
    /// <returns>The sum of elements 0 through <paramref name="size"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public long Naive(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // A new array is allocated on every call — the GC must eventually collect it.
        int[] data = new int[size];

        for (int i = 0; i < size; i++)
        {
            data[i] = i;
        }

        long sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += data[i];
        }

        return sum;
    }

    /// <summary>
    /// Reuses a pre-allocated instance buffer to avoid per-call heap allocation.
    /// Only sizes up to 1 024 are supported; larger inputs throw.
    /// </summary>
    /// <param name="size">
    /// The number of elements to process. Must be between 1 and 1 024 inclusive.
    /// </param>
    /// <returns>The sum of elements 0 through <paramref name="size"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero, negative, or exceeds the buffer length.
    /// </exception>
    public long Optimized(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        if (size > _buffer.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                $"Size must not exceed {_buffer.Length}. For larger inputs use ArrayPool<T>.");
        }

        // Write into the pre-allocated buffer — no heap allocation in this call.
        for (int i = 0; i < size; i++)
        {
            _buffer[i] = i;
        }

        long sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += _buffer[i];
        }

        return sum;
    }
}
