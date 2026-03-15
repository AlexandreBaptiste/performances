// ============================================================
// Concept  : Finalizers vs IDisposable
// Summary  : Contrasts the naive finalizer pattern with the proper IDisposable + GC.SuppressFinalize pattern
// When to use   : Use IDisposable for deterministic cleanup; use a finalizer only as a safety-net fallback
// When NOT to use: Do not rely solely on a finalizer for prompt resource cleanup — GC timing is non-deterministic
// ============================================================

namespace DotNet.Performance.Examples.GarbageCollector;

/// <summary>
/// Demonstrates a class that relies only on a finalizer for cleanup.
/// This is the <em>naive</em> pattern: cleanup is non-deterministic and incurs extra GC overhead
/// because the object must survive one extra collection to be finalized.
/// </summary>
public sealed class NaiveResource
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance was finalized by the GC.
    /// Used for demonstration/testing purposes only.
    /// </summary>
    public static bool WasFinalized { get; set; }

    /// <summary>
    /// Finalizer invoked by the GC when there are no more strong references to this instance.
    /// </summary>
    ~NaiveResource()
    {
        WasFinalized = true;
    }
}

/// <summary>
/// Demonstrates the recommended pattern: <see cref="IDisposable"/> with
/// <see cref="GC.SuppressFinalize"/> for deterministic cleanup, and a finalizer only as fallback.
/// </summary>
public sealed class ProperResource : IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Dispose"/> was called.
    /// </summary>
    public static bool WasDisposed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object was finalized without being disposed first.
    /// </summary>
    public static bool WasFinalizedAsFallback { get; set; }

    /// <summary>
    /// Releases managed resources deterministically and suppresses finalization
    /// so the GC does not need to run the finalizer.
    /// </summary>
    public void Dispose()
    {
        WasDisposed = true;

        // Tell the GC to skip the finalizer — the object is already cleaned up.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Safety-net finalizer that runs only if <see cref="Dispose"/> was never called.
    /// In production code this should release unmanaged resources.
    /// </summary>
    ~ProperResource()
    {
        WasFinalizedAsFallback = true;
    }
}

/// <summary>
/// Factory methods that create resource objects in different ways to illustrate the two patterns.
/// </summary>
public static class FinalizerDemo
{
    /// <summary>
    /// Creates a <see cref="NaiveResource"/> and returns without disposing it.
    /// The GC will eventually run the finalizer.
    /// </summary>
    public static void CreateAndForgetNaive()
    {
        NaiveResource _ = new NaiveResource();
    }

    /// <summary>
    /// Creates a <see cref="ProperResource"/> inside a <c>using</c> statement,
    /// ensuring <see cref="ProperResource.Dispose"/> is called deterministically.
    /// </summary>
    public static void CreateAndDisposeProper()
    {
        using ProperResource resource = new ProperResource();
    }
}
