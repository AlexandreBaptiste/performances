// ============================================================
// Concept  : Span<T> and Memory<T>
// Summary  : Demonstrates zero-copy array slicing with Memory<T> vs. a LINQ-based copy
// When to use   : When you need to pass an array slice to async code (Memory<T> survives await)
// When NOT to use: Span<T> is preferred for purely synchronous paths; Memory<T> for async ones
// ============================================================

namespace DotNet.Performance.Examples.SpanAndMemory;

/// <summary>
/// Compares a LINQ-based slice that creates a new heap-allocated copy against a
/// <see cref="Memory{T}"/>-based slice that views the original array without copying.
/// </summary>
/// <example>
/// <code>
/// var demo = new MemoryDemo();
/// int[] data = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
/// long withArray  = demo.SliceWithArray(data, 0, 5);  // allocates new int[5]
/// long withMemory = demo.SliceWithMemory(data, 0, 5); // zero-copy view
/// </code>
/// </example>
public sealed class MemoryDemo
{
    /// <summary>
    /// Creates a copy of the requested slice using LINQ <c>Skip/Take/ToArray</c>,
    /// allocating a new array, and returns the sum of its elements.
    /// </summary>
    /// <param name="data">The source array.</param>
    /// <param name="start">The zero-based start index of the slice.</param>
    /// <param name="length">The number of elements to include.</param>
    /// <returns>The sum of the selected elements.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public long SliceWithArray(int[] data, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Skip/Take/ToArray allocates a new heap array — an unnecessary copy.
        int[] slice = data.Skip(start).Take(length).ToArray();

        long sum = 0;

        for (int i = 0; i < slice.Length; i++)
        {
            sum += slice[i];
        }

        return sum;
    }

    /// <summary>
    /// Obtains a <see cref="Memory{T}"/> view of the requested slice without copying data,
    /// accesses it via <see cref="Memory{T}.Span"/>, and returns the sum of its elements.
    /// </summary>
    /// <param name="data">The source array.</param>
    /// <param name="start">The zero-based start index of the slice.</param>
    /// <param name="length">The number of elements to include.</param>
    /// <returns>The sum of the selected elements.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public long SliceWithMemory(int[] data, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(data);

        // AsMemory().Slice() is a struct wrapping a pointer and length — no copy, no allocation.
        Memory<int> memory = data.AsMemory().Slice(start, length);

        // .Span gives synchronous access; safe here since we do not cross an await boundary.
        ReadOnlySpan<int> span = memory.Span;

        long sum = 0;

        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }

        return sum;
    }
}
