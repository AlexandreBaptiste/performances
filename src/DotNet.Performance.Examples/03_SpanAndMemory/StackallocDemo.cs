// ============================================================
// Concept  : Span<T> and Memory<T>
// Summary  : Demonstrates using stackalloc to avoid heap allocation for small buffers
// When to use   : Fixed-size, small temporary buffers (≤ 256 bytes) that do not escape the method
// When NOT to use: Large buffers (risk of StackOverflowException), or buffers needed beyond method scope
// ============================================================

namespace DotNet.Performance.Examples.SpanAndMemory;

/// <summary>
/// Shows how <c>stackalloc</c> eliminates heap allocations for small, short-lived byte buffers.
/// The resulting <see cref="Span{T}"/> makes <c>stackalloc</c> memory-safe without the
/// <c>unsafe</c> keyword.
/// </summary>
/// <example>
/// <code>
/// var demo = new StackallocDemo();
/// long sumNaive     = demo.Naive(64);     // allocates byte[64] on the heap
/// long sumOptimized = demo.Optimized(64); // uses stackalloc — no GC involvement
/// </code>
/// </example>
public sealed class StackallocDemo
{
    /// <summary>Maximum buffer size allowed for stack allocation (prevents stack overflow).</summary>
    private const int MaxStackSize = 256;

    /// <summary>
    /// Allocates a <see cref="byte"/> array on the heap, fills it with values
    /// <c>i % 256</c> for <c>i</c> in [0, <paramref name="size"/>), and returns the sum.
    /// </summary>
    /// <param name="size">Number of bytes to allocate and sum. Must be positive.</param>
    /// <returns>Sum of the byte values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public long Naive(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // Heap allocation — the GC must eventually collect this array.
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
    /// Allocates a buffer on the current stack frame using <c>stackalloc</c>, fills it with
    /// values <c>i % 256</c> for <c>i</c> in [0, <paramref name="size"/>), and returns the sum.
    /// No GC involvement whatsoever — the memory is reclaimed when the method returns.
    /// </summary>
    /// <param name="size">
    /// Number of bytes to allocate and sum.
    /// Must be positive and at most <see cref="MaxStackSize"/> (256).
    /// </param>
    /// <returns>Sum of the byte values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero, negative, or exceeds <see cref="MaxStackSize"/>.
    /// </exception>
    public long Optimized(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        if (size > MaxStackSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                $"Size must not exceed {MaxStackSize} for stack allocation to be safe.");
        }

        // stackalloc: lives on the stack frame — zero GC involvement.
        Span<byte> buffer = stackalloc byte[size];

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
}
