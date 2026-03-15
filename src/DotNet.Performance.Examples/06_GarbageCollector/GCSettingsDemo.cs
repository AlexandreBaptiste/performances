// ============================================================
// Concept  : GCSettings — latency modes and server GC
// Summary  : Shows how to read and configure GC latency modes for latency-sensitive workloads
// When to use   : Real-time or latency-sensitive sections where GC pauses must be minimized
// When NOT to use: General-purpose code; always restore the original latency mode after your critical section
// ============================================================

using System.Runtime;

namespace DotNet.Performance.Examples.GarbageCollector;

/// <summary>
/// Demonstrates reading and adjusting <see cref="GCSettings"/> and experimenting
/// with no-GC regions for latency-critical sections.
/// </summary>
public static class GCSettingsDemo
{
    /// <summary>
    /// Returns the current <see cref="GCLatencyMode"/> configured for this process.
    /// </summary>
    /// <returns>The active <see cref="GCLatencyMode"/>.</returns>
    public static GCLatencyMode GetCurrentLatencyMode() => GCSettings.LatencyMode;

    /// <summary>
    /// Sets the GC latency mode to <see cref="GCLatencyMode.SustainedLowLatency"/>,
    /// which minimises GC pauses at the cost of higher memory usage.
    /// </summary>
    /// <remarks>
    /// Always restore the latency mode (e.g., to <see cref="GCLatencyMode.Interactive"/>)
    /// when the low-latency section is complete.
    /// </remarks>
    public static void SetSustainedLowLatency()
    {
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    /// <summary>
    /// Restores the GC latency mode to <see cref="GCLatencyMode.Interactive"/>, the default
    /// workstation GC mode.
    /// </summary>
    public static void RestoreInteractive()
    {
        GCSettings.LatencyMode = GCLatencyMode.Interactive;
    }

    /// <summary>
    /// Returns whether the process is using Server GC (<see langword="true"/>) or
    /// Workstation GC (<see langword="false"/>).
    /// </summary>
    /// <returns><see langword="true"/> if Server GC is active.</returns>
    public static bool IsServerGC() => GCSettings.IsServerGC;

    /// <summary>
    /// Attempts to enter a no-GC region with the specified memory budget.
    /// If successful, immediately ends the region and returns <see langword="true"/>.
    /// </summary>
    /// <param name="size">The memory budget for the no-GC region, in bytes.</param>
    /// <returns>
    /// <see langword="true"/> if <see cref="GC.TryStartNoGCRegion(long)"/> succeeded;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryNoGcRegion(long size)
    {
        bool started = GC.TryStartNoGCRegion(size);

        if (started)
        {
            GC.EndNoGCRegion();
        }

        return started;
    }
}
