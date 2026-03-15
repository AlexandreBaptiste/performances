// ============================================================
// Concept  : GC generations (Gen0, Gen1, Gen2)
// Summary  : Shows how object lifetime affects which GC generation collects it
// When to use   : Understanding allocation patterns and designing for Gen0 collection
// When NOT to use: Do not rely on GC collection counts for correctness logic (non-deterministic)
// ============================================================

namespace DotNet.Performance.Examples.GarbageCollector;

/// <summary>
/// Demonstrates how short-lived vs long-lived objects affect GC generation counts.
/// </summary>
public static class GCGenerationsDemo
{
    /// <summary>
    /// Returns the current GC collection counts for all three generations.
    /// </summary>
    /// <returns>
    /// A tuple of <c>(gen0, gen1, gen2)</c> collection counts at the time of the call.
    /// </returns>
    public static (int gen0, int gen1, int gen2) GetCollectionCounts()
    {
        return (
            GC.CollectionCount(0),
            GC.CollectionCount(1),
            GC.CollectionCount(2)
        );
    }

    /// <summary>
    /// Allocates <paramref name="count"/> short-lived <see cref="byte"/> arrays of 100 bytes each,
    /// forces a GC collection, and returns the increase in Gen0 collection count.
    /// </summary>
    /// <param name="count">Number of short-lived arrays to allocate.</param>
    /// <returns>The number of additional Gen0 collections that occurred.</returns>
    public static int AllocateShortLivedObjects(int count)
    {
        int before = GC.CollectionCount(0);

        for (int i = 0; i < count; i++)
        {
            // Each array is immediately eligible for collection — stays in Gen0.
            byte[] _ = new byte[100];
        }

        GC.Collect(0, GCCollectionMode.Forced, blocking: true);

        return GC.CollectionCount(0) - before;
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> holding <paramref name="count"/> <see cref="byte"/> arrays
    /// of 1000 bytes each, retaining them long enough to survive Gen0 and promote to Gen1/Gen2,
    /// then clears the list and forces a full GC.
    /// </summary>
    /// <param name="count">Number of long-lived arrays to allocate.</param>
    /// <returns>The count of arrays that were retained in the list before clearing.</returns>
    public static int AllocateLongLivedObjects(int count)
    {
        List<byte[]> longLived = new List<byte[]>(count);

        for (int i = 0; i < count; i++)
        {
            longLived.Add(new byte[1000]);
        }

        // Survive at least one Gen0 collection to promote to Gen1/Gen2.
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        GC.Collect(1, GCCollectionMode.Forced, blocking: true);

        int retained = longLived.Count;

        longLived.Clear();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

        return retained;
    }
}
