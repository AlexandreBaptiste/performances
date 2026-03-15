// ============================================================
// Concept  : SearchValues<T> for Character Lookup
// Summary  : Contrasts IndexOfAny with an ad-hoc char array vs a pre-built SearchValues<char>
// When to use   : Repeated searches for a fixed set of characters in text-processing hot paths
// When NOT to use: One-time or rare searches where the SearchValues construction cost is not amortised
// ============================================================

using System.Buffers;

namespace DotNet.Performance.Examples.StringOptimization;

/// <summary>
/// Demonstrates <see cref="SearchValues{T}"/> (<c>System.Buffers</c>), introduced in .NET 8,
/// as a high-performance alternative to <see cref="string.IndexOfAny(char[])"/> for searching
/// a fixed, known set of characters.
/// </summary>
public static class SearchValuesDemo
{
    /// <summary>
    /// Pre-built, read-only set of vowel characters optimised for repeated searches.
    /// <see cref="SearchValues.Create(ReadOnlySpan{char})"/> performs one-time preprocessing
    /// (e.g. building lookup bitmaps) so that each search is maximally fast.
    /// </summary>
    private static readonly SearchValues<char> _vowels =
        SearchValues.Create("aeiouAEIOU");

    /// <summary>
    /// Returns the index of the first vowel in <paramref name="input"/> by calling
    /// <see cref="string.IndexOfAny(char[])"/> with an inline <c>char[]</c> literal.
    /// A new array is allocated on every call and no preprocessing is performed.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <returns>
    /// The zero-based index of the first vowel, or <c>-1</c> if none is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    public static int IndexOfVowelNaive(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.IndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' });
    }

    /// <summary>
    /// Returns the index of the first vowel in <paramref name="input"/> using the
    /// pre-built <see cref="_vowels"/> <see cref="SearchValues{T}"/> instance.
    /// Zero heap allocations on every call; the preprocessing is amortised at startup.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <returns>
    /// The zero-based index of the first vowel, or <c>-1</c> if none is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
    public static int IndexOfVowelOptimized(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.AsSpan().IndexOfAny(_vowels);
    }
}
