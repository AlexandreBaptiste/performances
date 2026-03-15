// ============================================================
// Concept  : CollectionsMarshal.AsSpan for List<T>
// Summary  : Bypasses List<T> enumerator overhead by obtaining a direct Span<T> over its backing array
// When to use   : Read-heavy loops over List<T> on hot paths where every allocation counts
// When NOT to use: When the list is modified during the loop — the span becomes invalid on resize
// ============================================================

using System.Runtime.InteropServices;

namespace DotNet.Performance.Examples.Patterns;

/// <summary>
/// Compares summing a <see cref="List{T}"/> via LINQ against accessing its backing array directly
/// through <see cref="CollectionsMarshal.AsSpan{T}"/>.
/// <para>
/// LINQ allocates an enumerator and iterates via the <c>IEnumerable&lt;T&gt;</c> interface, adding
/// virtual-dispatch and allocation overhead. <see cref="CollectionsMarshal.AsSpan{T}"/> returns a
/// <see cref="Span{T}"/> over the internal array, allowing a zero-allocation bounds-checked loop.
/// </para>
/// </summary>
public static class CollectionsMarshalDemo
{
    /// <summary>
    /// Sums all elements of <paramref name="data"/> using LINQ's <c>Sum</c> extension method.
    /// </summary>
    /// <param name="data">Input list of integers.</param>
    /// <returns>Sum of all elements as a <see cref="long"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long SumWithLinq(List<int> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return data.Sum(x => (long)x);
    }

    /// <summary>
    /// Sums all elements of <paramref name="data"/> by obtaining a <see cref="Span{T}"/> over the
    /// list's backing array via <see cref="CollectionsMarshal.AsSpan{T}"/> and iterating without
    /// any heap allocation.
    /// </summary>
    /// <param name="data">Input list of integers.</param>
    /// <returns>Sum of all elements as a <see cref="long"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long SumWithMarshal(List<int> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Span<int> span = CollectionsMarshal.AsSpan(data);
        long sum = 0;
        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }
}
