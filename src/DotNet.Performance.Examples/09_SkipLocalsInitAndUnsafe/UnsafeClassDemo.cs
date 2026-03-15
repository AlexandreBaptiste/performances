// ============================================================
// Concept  : System.Runtime.CompilerServices.Unsafe
// Summary  : Low-level pointer arithmetic via Unsafe.Add and Unsafe.SizeOf for zero-overhead memory ops
// When to use   : Tightly optimised inner loops where bounds are manually verified and performance is critical
// When NOT to use: Any public API where bounds are not externally verified — risk of memory corruption
// ============================================================

// UNSAFE — educational only
using System.Runtime.CompilerServices;

namespace DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;

/// <summary>
/// Demonstrates <see cref="Unsafe"/> helpers for low-level pointer arithmetic without heap allocation.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Unsafe.Add{T}(ref T, int)"/> advances a managed reference by a given number of elements —
/// equivalent to pointer arithmetic but without emitting unsafe IL from the call site.
/// </para>
/// <para>
/// <see cref="Unsafe.SizeOf{T}"/> returns the JIT-resolved sizeof a struct without boxing or reflection.
/// </para>
/// <para>
/// <strong>Caution:</strong> these APIs bypass runtime bounds checks; always verify index ranges manually.
/// </para>
/// </remarks>
public static class UnsafeClassDemo
{
    /// <summary>
    /// Copies elements from <paramref name="source"/> into <paramref name="destination"/>,
    /// starting at <paramref name="offset"/> in the source, using <see cref="Unsafe.Add{T}(ref T, int)"/>.
    /// </summary>
    /// <param name="source">The source array.</param>
    /// <param name="destination">The destination array.</param>
    /// <param name="offset">Zero-based start index in <paramref name="source"/>.</param>
    /// <returns>
    /// The number of elements copied:
    /// <c>min(source.Length - offset, destination.Length)</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="source"/> or <paramref name="destination"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> is negative or greater than or equal to
    /// <c>source.Length</c>.
    /// </exception>
    public static int CopyWithOffset(int[] source, int[] destination, int offset)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, source.Length);

        int count = Math.Min(source.Length - offset, destination.Length);

        // Obtain a managed reference to the first element, then advance by offset.
        // This avoids a bounds check on each iteration while remaining verifiably safe.
        ref int src = ref source[0];
        ref int srcAtOffset = ref Unsafe.Add(ref src, offset);

        for (int i = 0; i < count; i++)
        {
            destination[i] = Unsafe.Add(ref srcAtOffset, i);
        }

        return count;
    }

    /// <summary>
    /// Returns the managed size in bytes of <typeparamref name="T"/>,
    /// bypassing boxing or reflection via <see cref="Unsafe.SizeOf{T}"/>.
    /// </summary>
    /// <typeparam name="T">A struct type whose size to measure.</typeparam>
    /// <returns>The size of <typeparamref name="T"/> in bytes, as reported by the JIT.</returns>
    public static int GetByteSize<T>() where T : struct => Unsafe.SizeOf<T>();
}
