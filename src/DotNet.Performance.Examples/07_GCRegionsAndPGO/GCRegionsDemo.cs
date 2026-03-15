// ============================================================
// Concept  : GC.TryStartNoGCRegion — latency-critical sections
// Summary  : Demonstrates running a work block inside a no-GC region to prevent GC pauses
// When to use   : Short, latency-critical sections where a GC pause would be unacceptable
// When NOT to use: Long-running operations; the CLR will abort the region if the budget is exceeded
// ============================================================

namespace DotNet.Performance.Examples.GCRegionsAndPGO;

/// <summary>
/// Demonstrates wrapping a critical section in a no-GC region using
/// <see cref="GC.TryStartNoGCRegion(long)"/> and comparing it with an unguarded execution.
/// </summary>
public static class GCRegionsDemo
{
    private const long DefaultBudgetBytes = 4_194_304; // 4 MiB

    /// <summary>
    /// Runs <paramref name="work"/> inside a no-GC region, preventing GC collections for the
    /// duration of the critical section (up to <paramref name="budgetBytes"/> of allocations).
    /// </summary>
    /// <param name="work">The delegate to execute without GC interruption.</param>
    /// <param name="budgetBytes">
    /// Memory budget for the no-GC region in bytes. Defaults to 4 MiB.
    /// The CLR will abort the region if this budget is exceeded.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="work"/> is <see langword="null"/>.
    /// </exception>
    public static void RunCriticalSection(Action work, long budgetBytes = DefaultBudgetBytes)
    {
        ArgumentNullException.ThrowIfNull(work);

        bool started = GC.TryStartNoGCRegion(budgetBytes);

        try
        {
            work();
        }
        finally
        {
            if (started)
            {
                GC.EndNoGCRegion();
            }
        }
    }

    /// <summary>
    /// Runs <paramref name="work"/> directly without any no-GC region, for comparison.
    /// </summary>
    /// <param name="work">The delegate to execute.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="work"/> is <see langword="null"/>.
    /// </exception>
    public static void RunWithoutNoGCRegion(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        work();
    }
}
