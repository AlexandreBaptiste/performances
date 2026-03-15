// ============================================================
// Concept  : FrozenDictionary<TKey,TValue> and FrozenHashSet<T>
// Summary  : Read-only, highly-optimised collections built once and never modified
// When to use   : Configuration tables, lookup dictionaries built once at startup and read thousands of times
// When NOT to use: Collections that must be mutated after construction
// ============================================================

using System.Collections.Frozen;

namespace DotNet.Performance.Examples.GCRegionsAndPGO;

/// <summary>
/// Demonstrates building and querying <see cref="FrozenDictionary{TKey,TValue}"/> and
/// <see cref="FrozenSet{T}"/> alongside their mutable counterparts.
/// </summary>
public static class FrozenCollectionsDemo
{
    /// <summary>
    /// Creates a <see cref="FrozenDictionary{TKey,TValue}"/> from the given key-value pairs.
    /// The resulting dictionary is immutable and optimised for read-heavy workloads.
    /// </summary>
    /// <param name="pairs">The source key-value pairs.</param>
    /// <returns>A frozen, read-optimised dictionary.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="pairs"/> is <see langword="null"/>.
    /// </exception>
    public static FrozenDictionary<string, int> CreateFrozenDictionary(
        IEnumerable<KeyValuePair<string, int>> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);
        return pairs.ToFrozenDictionary();
    }

    /// <summary>
    /// Creates a <see cref="FrozenSet{T}"/> from the given string items.
    /// </summary>
    /// <param name="items">The source strings.</param>
    /// <returns>A frozen, read-optimised hash set.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="items"/> is <see langword="null"/>.
    /// </exception>
    public static FrozenSet<string> CreateFrozenHashSet(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return items.ToFrozenSet();
    }

    /// <summary>
    /// Looks up <paramref name="key"/> in a mutable <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="dict">The source dictionary.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value, or <c>0</c> if not found.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dict"/> or <paramref name="key"/> is <see langword="null"/>.
    /// </exception>
    public static int LookupNaive(Dictionary<string, int> dict, string key)
    {
        ArgumentNullException.ThrowIfNull(dict);
        ArgumentNullException.ThrowIfNull(key);

        dict.TryGetValue(key, out int value);
        return value;
    }

    /// <summary>
    /// Looks up <paramref name="key"/> in a <see cref="FrozenDictionary{TKey,TValue}"/>.
    /// Frozen dictionaries use a specialised hash strategy for faster reads.
    /// </summary>
    /// <param name="dict">The frozen source dictionary.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value, or <c>0</c> if not found.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dict"/> or <paramref name="key"/> is <see langword="null"/>.
    /// </exception>
    public static int LookupFrozen(FrozenDictionary<string, int> dict, string key)
    {
        ArgumentNullException.ThrowIfNull(dict);
        ArgumentNullException.ThrowIfNull(key);

        dict.TryGetValue(key, out int value);
        return value;
    }
}
