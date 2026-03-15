// ============================================================
// Concept  : MethodImpl(AggressiveInlining)
// Summary  : Hints to the JIT to inline a small method at all call sites to eliminate call overhead
// When to use   : Tiny, hot methods whose call overhead is measurable relative to their body
// When NOT to use: Large methods or rarely-called code paths — inlining them bloats the call site
// ============================================================

using System.Runtime.CompilerServices;

namespace DotNet.Performance.Examples.Inlining;

/// <summary>
/// Compares a small mathematical computation with and without
/// <see cref="MethodImplOptions.AggressiveInlining"/>.
/// </summary>
public static class AggressiveInliningDemo
{
    /// <summary>
    /// Computes <c>x² + 2x + 1</c> <em>without</em> an inlining hint.
    /// The JIT may or may not inline this method based on its own heuristics.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The result of <c>x² + 2x + 1</c>.</returns>
    public static int Naive(int x) => x * x + 2 * x + 1;

    /// <summary>
    /// Computes <c>x² + 2x + 1</c> <em>with</em> <see cref="MethodImplOptions.AggressiveInlining"/>,
    /// instructing the JIT to always inline this call site and eliminate the method-call overhead.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The result of <c>x² + 2x + 1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Optimized(int x) => x * x + 2 * x + 1;
}
