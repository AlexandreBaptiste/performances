// ============================================================
// Concept  : Span<T> and Memory<T>
// Summary  : Demonstrates zero-allocation string slicing using AsSpan() vs. Substring()
// When to use   : Parsing, slicing, or processing sub-regions of strings without allocating
// When NOT to use: When the resulting string must be stored long-term (a new string is still needed)
// ============================================================

namespace DotNet.Performance.Examples.SpanAndMemory;

/// <summary>
/// Shows how to slice and trim a portion of a string using <see cref="ReadOnlySpan{T}"/>
/// instead of <see cref="string.Substring(int,int)"/>, reducing intermediate heap allocations.
/// </summary>
/// <example>
/// <code>
/// var demo = new SpanDemo();
/// string naive     = demo.Naive("Hello, World!", 0, 5);     // allocates intermediate string
/// string optimized = demo.Optimized("Hello, World!", 0, 5); // span slice, one final allocation
/// </code>
/// </example>
public sealed class SpanDemo
{
    /// <summary>
    /// Slices a substring using <see cref="string.Substring(int,int)"/>, which always allocates
    /// a new heap-managed string for the slice before trimming (a second allocation).
    /// </summary>
    /// <param name="input">The source string.</param>
    /// <param name="start">Zero-based start index of the slice.</param>
    /// <param name="length">Number of characters to include.</param>
    /// <returns>The trimmed substring.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <see langword="null"/>.
    /// </exception>
    public string Naive(string input, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Substring allocates a new string, then Trim() may allocate another.
        return input.Substring(start, length).Trim();
    }

    /// <summary>
    /// Slices the string using <see cref="MemoryExtensions.AsSpan(string)"/> to avoid the
    /// intermediate allocation. The <c>Trim()</c> on a <see cref="ReadOnlySpan{T}"/>
    /// adjusts only the pointer and length — no allocation. Only one heap allocation occurs
    /// when converting the final span back to a managed <see cref="string"/>.
    /// </summary>
    /// <param name="input">The source string.</param>
    /// <param name="start">Zero-based start index of the slice.</param>
    /// <param name="length">Number of characters to include.</param>
    /// <returns>The trimmed substring.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <see langword="null"/>.
    /// </exception>
    public string Optimized(string input, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(input);

        // AsSpan().Slice() is a pointer + length adjustment — no heap allocation.
        // Trim() on ReadOnlySpan<char> likewise only narrows the view.
        ReadOnlySpan<char> span = input.AsSpan().Slice(start, length).Trim();

        // One unavoidable allocation: converting the span back to a managed string.
        return new string(span);
    }
}
