// ============================================================
// Concept  : StringBuilder Object Pooling
// Summary  : Reuses StringBuilder instances from a SimpleObjectPool to eliminate repeated construction
// When to use   : High-frequency formatting loops where StringBuilder construction cost is measurable
// When NOT to use: Single-shot or low-frequency formatting where pool bookkeeping adds complexity
// ============================================================

using System.Text;

namespace DotNet.Performance.Examples.ObjectPooling;

/// <summary>
/// Compares creating a new <see cref="StringBuilder"/> on every formatting iteration
/// against renting one from a <see cref="SimpleObjectPool{T}"/> and returning it after use.
/// </summary>
public sealed class StringBuilderPoolDemo
{
    private static readonly SimpleObjectPool<StringBuilder> _pool =
        new SimpleObjectPool<StringBuilder>(() => new StringBuilder());

    /// <summary>
    /// Formats <paramref name="template"/> appended with the iteration index for each of
    /// the <paramref name="iterations"/>, allocating a new <see cref="StringBuilder"/> each time.
    /// </summary>
    /// <param name="template">Base string to format.</param>
    /// <param name="iterations">Number of formatting iterations to perform.</param>
    /// <returns>Total count of formatted results (equal to <paramref name="iterations"/>).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="template"/> is null.</exception>
    public static int FormatWithNew(string template, int iterations)
    {
        ArgumentNullException.ThrowIfNull(template);

        int count = 0;
        for (int i = 0; i < iterations; i++)
        {
            var sb = new StringBuilder();
            sb.Append(template).Append(i);
            string _ = sb.ToString();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Formats <paramref name="template"/> appended with the iteration index for each of
    /// the <paramref name="iterations"/>, renting a pooled <see cref="StringBuilder"/> and
    /// returning it after each iteration to avoid repeated allocations.
    /// </summary>
    /// <param name="template">Base string to format.</param>
    /// <param name="iterations">Number of formatting iterations to perform.</param>
    /// <returns>Total count of formatted results (equal to <paramref name="iterations"/>).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="template"/> is null.</exception>
    public static int FormatWithPool(string template, int iterations)
    {
        ArgumentNullException.ThrowIfNull(template);

        int count = 0;
        for (int i = 0; i < iterations; i++)
        {
            StringBuilder sb = _pool.Rent();
            try
            {
                sb.Clear();
                sb.Append(template).Append(i);
                string _ = sb.ToString();
                count++;
            }
            finally
            {
                _pool.Return(sb);
            }
        }
        return count;
    }
}
