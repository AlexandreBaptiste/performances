// ============================================================
// Concept  : JIT Auto-Vectorization
// Summary  : Shows code patterns that the JIT can (and cannot) auto-vectorize
// When to use   : Simple forward loops with no data dependencies or complex branches
// When NOT to use: Loops with cross-iteration dependencies or conditionals on the index
// ============================================================

namespace DotNet.Performance.Examples.SIMD;

/// <summary>
/// Demonstrates the difference between loop shapes that the JIT can auto-vectorize
/// and those it cannot, both producing the same numeric result.
/// </summary>
public static class AutoVectorizationDemo
{
    /// <summary>
    /// Sums all elements of <paramref name="data"/> with a simple forward loop.
    /// The absence of conditionals and cross-iteration dependencies allows the JIT to
    /// emit SIMD instructions automatically (auto-vectorization).
    /// </summary>
    /// <param name="data">Input array of integers.</param>
    /// <returns>Sum of all elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long VectorizationFriendlySum(int[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        long sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }
        return sum;
    }

    /// <summary>
    /// Sums all elements of <paramref name="data"/>, splitting accumulation into separate
    /// even and odd buckets via an index parity check.
    /// Although the final result is identical to <see cref="VectorizationFriendlySum"/>,
    /// the branch on <c>i % 2</c> introduces a data-dependent conditional that prevents
    /// straightforward auto-vectorization by the JIT.
    /// </summary>
    /// <param name="data">Input array of integers.</param>
    /// <returns>Sum of all elements; same value as <see cref="VectorizationFriendlySum"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long VectorizationUnfriendlySum(int[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        long evenSum = 0;
        long oddSum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (i % 2 == 0)
            {
                evenSum += data[i];
            }
            else
            {
                oddSum += data[i];
            }
        }
        return evenSum + oddSum;
    }
}
