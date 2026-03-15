// ============================================================
// Concept  : ArrayPool<T> and MemoryPool<T>
// Summary  : Demonstrates MemoryPool<T> for slice-oriented, async-compatible pool-backed buffers
// When to use   : When you need Memory<T> semantics (async-safe) with pool-backed allocation
// When NOT to use: Synchronous scenarios where ArrayPool<T> is simpler and carries less overhead
// ============================================================

using System.Buffers;

namespace DotNet.Performance.Examples.ArrayPool;

/// <summary>
/// Compares a standard <c>new byte[]</c> allocation against renting an
/// <see cref="IMemoryOwner{T}"/> from <see cref="MemoryPool{T}.Shared"/>.
/// <see cref="Memory{T}"/> is the preferred buffer abstraction for async code because,
/// unlike <see cref="Span{T}"/>, it can survive across <c>await</c> boundaries.
/// </summary>
/// <example>
/// <code>
/// var demo = new MemoryPoolDemo();
/// byte csNew  = demo.ProcessWithNew(256);        // fresh heap array
/// byte csPool = demo.ProcessWithMemoryPool(256); // pool-backed Memory&lt;byte&gt;
/// </code>
/// </example>
public sealed class MemoryPoolDemo
{
    /// <summary>
    /// Allocates a fresh <see cref="byte"/> array on the heap, fills it with the standard
    /// pattern, and returns a checksum (XOR of all bytes).
    /// </summary>
    /// <param name="size">Number of bytes to process. Must be positive.</param>
    /// <returns>XOR checksum of all bytes in the filled buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public byte ProcessWithNew(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        byte[] buffer = new byte[size];

        FillBuffer(buffer.AsSpan(0, size));

        return ComputeChecksum(buffer.AsSpan(0, size));
    }

    /// <summary>
    /// Rents an <see cref="IMemoryOwner{T}"/> from <see cref="MemoryPool{T}.Shared"/>,
    /// processes the buffer, and disposes the owner in a <c>finally</c> block so the
    /// memory is returned to the pool reliably.
    /// </summary>
    /// <param name="size">Number of bytes to process. Must be positive.</param>
    /// <returns>XOR checksum of all bytes in the filled buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is zero or negative.
    /// </exception>
    public byte ProcessWithMemoryPool(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        // IMemoryOwner<T> wraps the rented Memory<T> and returns it to the pool on Dispose().
        IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(size);

        try
        {
            // Slice to exactly `size` elements; rented memory may be larger.
            Memory<byte> memory = owner.Memory.Slice(0, size);
            Span<byte> span = memory.Span;

            FillBuffer(span);

            return ComputeChecksum(span);
        }
        finally
        {
            // Dispose() returns the buffer to the pool even if an exception propagates.
            owner.Dispose();
        }
    }

    private static void FillBuffer(Span<byte> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = (byte)(i % 256);
        }
    }

    private static byte ComputeChecksum(ReadOnlySpan<byte> span)
    {
        byte checksum = 0;

        for (int i = 0; i < span.Length; i++)
        {
            checksum ^= span[i];
        }

        return checksum;
    }
}
