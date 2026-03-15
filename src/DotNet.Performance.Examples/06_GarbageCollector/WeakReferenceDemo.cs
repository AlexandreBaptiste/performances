// ============================================================
// Concept  : WeakReference<T> for cache-friendly references
// Summary  : Demonstrates tracking objects without preventing garbage collection
// When to use   : Caches, event handlers, or other cases where you want to observe an object without owning it
// When NOT to use: When you need a guaranteed non-null reference; use a strong reference instead
// ============================================================

namespace DotNet.Performance.Examples.GarbageCollector;

/// <summary>
/// Demonstrates using <see cref="WeakReference{T}"/> to hold a reference to an object
/// that can still be garbage-collected when no strong references exist.
/// </summary>
public static class WeakReferenceDemo
{
    /// <summary>
    /// Creates a <see cref="WeakReference{T}"/> wrapping the provided <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The string to wrap. Must not be <see langword="null"/>.</param>
    /// <returns>A weak reference to <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public static WeakReference<string> CreateWeakRef(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new WeakReference<string>(value);
    }

    /// <summary>
    /// Attempts to retrieve the target of a <see cref="WeakReference{T}"/>.
    /// </summary>
    /// <param name="weakRef">The weak reference to query.</param>
    /// <param name="value">
    /// When this method returns <see langword="true"/>, contains the target object;
    /// otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the target is still alive; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="weakRef"/> is <see langword="null"/>.
    /// </exception>
    public static bool TryGetValue(WeakReference<string> weakRef, out string? value)
    {
        ArgumentNullException.ThrowIfNull(weakRef);
        return weakRef.TryGetTarget(out value);
    }

    /// <summary>
    /// Returns whether the target of the weak reference is still alive (not yet collected).
    /// </summary>
    /// <param name="weakRef">The weak reference to check.</param>
    /// <returns><see langword="true"/> if the target has not been collected.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="weakRef"/> is <see langword="null"/>.
    /// </exception>
    public static bool IsAlive(WeakReference<string> weakRef)
    {
        ArgumentNullException.ThrowIfNull(weakRef);
        return weakRef.TryGetTarget(out _);
    }
}
