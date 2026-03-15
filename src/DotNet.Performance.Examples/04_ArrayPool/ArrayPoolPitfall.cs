// ============================================================
// Concept  : ArrayPool<T> and MemoryPool<T>
// Summary  : Illustrates the common pitfall of forgetting to return a rented ArrayPool buffer
// When to use   : As an educational reference — LeakyRent must NEVER appear in production code
// When NOT to use: LeakyRent is intentionally broken; always use the CorrectRent pattern
// ============================================================

using System.Buffers;

namespace DotNet.Performance.Examples.ArrayPool;

/// <summary>
/// Demonstrates the "leaky rent" anti-pattern alongside the correct <c>try/finally</c> fix.
/// </summary>
/// <remarks>
/// <para>
/// <strong>WARNING — <see cref="LeakyRent"/>:</strong> This method intentionally rents a buffer
/// from <see cref="ArrayPool{T}"/> and never returns it. Over time this causes the pool to grow
/// unboundedly, negates all pooling benefits, and increases GC pressure — exactly the opposite
/// of what pooling is supposed to achieve.
/// </para>
/// <para>
/// <strong>CORRECT pattern — <see cref="CorrectRent"/>:</strong> Always place
/// <c>ArrayPool&lt;T&gt;.Shared.Return(buffer)</c> inside a <c>finally</c> block so the buffer
/// is released even if an exception is thrown.
/// </para>
/// </remarks>
public sealed class ArrayPoolPitfall
{
    /// <summary>
    /// <strong>BAD — Educational purposes only. Do not use in production.</strong>
    /// <para>
    /// Rents a buffer from <see cref="ArrayPool{T}.Shared"/> but never returns it.
    /// The pool cannot reuse the buffer; each call effectively leaks memory from the pool's
    /// perspective, causing it to allocate new arrays indefinitely.
    /// </para>
    /// </summary>
    /// <param name="size">Number of bytes to rent. Must be positive.</param>
    /// <returns>Sum of byte values in the rented (and leaked) buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    [Obsolete("LeakyRent intentionally leaks the rented buffer. Use CorrectRent instead.")]
    public long LeakyRent(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // BAD: The buffer is rented but never returned to the pool.
        // Each call steals a buffer permanently; the pool keeps allocating new ones.
        byte[] buffer = ArrayPool<byte>.Shared.Rent(size);

        for (int i = 0; i < size; i++)
        {
            buffer[i] = (byte)(i % 256);
        }

        long sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += buffer[i];
        }

        // MISSING: ArrayPool<byte>.Shared.Return(buffer);
        // Without this line, the buffer is permanently lost from the pool.
        return sum;
    }

    /// <summary>
    /// <strong>GOOD — Use this pattern in all production code.</strong>
    /// <para>
    /// Rents a buffer, uses it within a <c>try</c> block, and returns it in the <c>finally</c>
    /// block to guarantee the pool always gets its buffer back — regardless of exceptions.
    /// </para>
    /// </summary>
    /// <param name="size">Number of bytes to rent. Must be positive.</param>
    /// <returns>Sum of byte values in the rented buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public long CorrectRent(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        byte[] buffer = ArrayPool<byte>.Shared.Rent(size);

        try
        {
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
            // GOOD: The buffer is unconditionally returned to the pool.
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
