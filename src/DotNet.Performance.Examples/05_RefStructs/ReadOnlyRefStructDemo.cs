// ============================================================
// Concept  : readonly ref struct for zero-copy string views
// Summary  : Wraps a ReadOnlySpan<char> in a readonly ref struct to provide a zero-allocation string slice
// When to use   : When you need an immutable, stack-only view over an existing string or char buffer
// When NOT to use: When the view must escape the current stack frame or be stored long-term
// ============================================================

namespace DotNet.Performance.Examples.RefStructs;

/// <summary>
/// An immutable, stack-only view over a slice of a <see cref="string"/> (or any <see cref="char"/> memory).
/// </summary>
/// <remarks>
/// Being a <c>readonly ref struct</c> means all fields are implicitly read-only after construction
/// and the same stack-only restrictions as <c>ref struct</c> apply.
/// </remarks>
public readonly ref struct ReadOnlyView
{
    /// <summary>Gets the read-only span of characters in this view.</summary>
    public ReadOnlySpan<char> Data { get; init; }
}

/// <summary>
/// Demonstrates creating and querying a <see cref="ReadOnlyView"/> without heap allocation.
/// </summary>
public static class ReadOnlyRefStructDemo
{
    /// <summary>
    /// Creates a <see cref="ReadOnlyView"/> over a slice of the given string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="start">Zero-based start index of the slice.</param>
    /// <param name="length">Number of characters in the slice.</param>
    /// <returns>A <see cref="ReadOnlyView"/> wrapping the requested slice.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static ReadOnlyView CreateView(string source, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(source);

        // AsSpan().Slice() is a pointer + length adjustment — no allocation.
        return new ReadOnlyView { Data = source.AsSpan().Slice(start, length) };
    }

    /// <summary>
    /// Returns the number of characters in the view.
    /// </summary>
    /// <param name="view">The view to inspect.</param>
    /// <returns>The length of <see cref="ReadOnlyView.Data"/>.</returns>
    public static int GetLength(ReadOnlyView view) => view.Data.Length;

    /// <summary>
    /// Returns whether <paramref name="view"/> contains the specified character.
    /// </summary>
    /// <param name="view">The view to search.</param>
    /// <param name="target">The character to look for.</param>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
    public static bool Contains(ReadOnlyView view, char target) => view.Data.Contains(target);
}
