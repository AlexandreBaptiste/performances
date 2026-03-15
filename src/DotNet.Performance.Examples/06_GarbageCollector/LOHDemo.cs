// ============================================================
// Concept  : Large Object Heap (LOH)
// Summary  : Demonstrates the LOH threshold and how large allocations bypass the SOH
// When to use   : Understanding memory layout when allocating buffers ≥ 85,000 bytes
// When NOT to use: Do not allocate on the LOH unnecessarily — LOH is only collected during full GC
// ============================================================

namespace DotNet.Performance.Examples.GarbageCollector;

/// <summary>
/// Demonstrates the Large Object Heap (LOH) threshold and how to check whether an
/// allocation would end up on the LOH vs the Small Object Heap (SOH).
/// </summary>
public static class LOHDemo
{
    /// <summary>
    /// The minimum allocation size (in bytes) that causes the GC to use the Large Object Heap.
    /// Objects of this size or larger skip Gen0/Gen1 promotion and are placed directly on the LOH.
    /// </summary>
    public const int LohThresholdBytes = 85_000;

    /// <summary>
    /// Allocates a <see cref="byte"/> array of exactly <see cref="LohThresholdBytes"/> bytes,
    /// which will be placed on the LOH by the GC.
    /// </summary>
    /// <returns>The length of the allocated array.</returns>
    public static int AllocateLargeObject()
    {
        byte[] largeObj = new byte[LohThresholdBytes];
        return largeObj.Length;
    }

    /// <summary>
    /// Allocates a <see cref="byte"/> array of 1 000 bytes,
    /// which stays on the Small Object Heap (SOH).
    /// </summary>
    /// <returns>The length of the allocated array.</returns>
    public static int AllocateSmallObject()
    {
        byte[] smallObj = new byte[1_000];
        return smallObj.Length;
    }

    /// <summary>
    /// Returns whether an allocation of <paramref name="sizeInBytes"/> bytes would land on the LOH.
    /// </summary>
    /// <param name="sizeInBytes">The intended allocation size in bytes.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="sizeInBytes"/> is at or above <see cref="LohThresholdBytes"/>;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsOnLoh(int sizeInBytes) => sizeInBytes >= LohThresholdBytes;
}
