// ============================================================
// Concept  : MethodImpl(AggressiveOptimization)
// Summary  : Requests full JIT optimisation (Tier-2) immediately, skipping profiling phases
// When to use   : Methods that are known to be hot from the start and should not wait for Tier-1 promotion
// When NOT to use: Most methods — let the JIT decide; over-use can slow startup and waste compilation resources
// ============================================================

using System.Runtime.CompilerServices;

namespace DotNet.Performance.Examples.Inlining;

/// <summary>
/// Contrasts summing an integer array with and without
/// <see cref="MethodImplOptions.AggressiveOptimization"/>.
/// </summary>
public static class AggressiveOptimizationDemo
{
    /// <summary>
    /// Sums all elements of <paramref name="data"/> without an aggressive optimisation hint.
    /// The JIT will promote this to Tier-1 naturally after observing hot-path behaviour.
    /// </summary>
    /// <param name="data">The array to sum. Must not be <see langword="null"/>.</param>
    /// <returns>The sum of all elements.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public static long NaiveSum(int[] data)
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
    /// Sums all elements of <paramref name="data"/> with <see cref="MethodImplOptions.AggressiveOptimization"/>,
    /// requesting that the JIT apply full optimisations (Tier-2) on first compilation without waiting
    /// for profiling feedback.
    /// </summary>
    /// <param name="data">The array to sum. Must not be <see langword="null"/>.</param>
    /// <returns>The sum of all elements.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static long OptimizedSum(int[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        long sum = 0;

        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }

        return sum;
    }
}
