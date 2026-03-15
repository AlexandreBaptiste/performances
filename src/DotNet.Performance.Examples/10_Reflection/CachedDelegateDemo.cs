// ============================================================
// Concept  : Cached reflection delegate
// Summary  : Creates a strongly-typed delegate from MethodInfo once and reuses it — near-native call speed
// When to use   : Generic frameworks that need property access by name but can cache per-type metadata once
// When NOT to use: One-call scenarios where the caching overhead exceeds the savings; prefer source generators for AOT
// ============================================================

using System.Reflection;

namespace DotNet.Performance.Examples.Reflection;

/// <summary>
/// Demonstrates caching a <see cref="Delegate"/> created from <see cref="MethodInfo"/> to eliminate
/// per-call reflection overhead.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Delegate.CreateDelegate(Type, MethodInfo)"/> compiles the <see cref="MethodInfo"/>
/// into a strongly-typed delegate once at class initialisation.
/// </para>
/// <para>
/// Subsequent calls to <see cref="GetValueCached"/> go through the delegate with negligible overhead —
/// comparable to a direct virtual call — because the JIT can devirtualise and even inline it.
/// </para>
/// </remarks>
public static class CachedDelegateDemo
{
    private static readonly Func<SampleEntity, int> _getter;

    static CachedDelegateDemo()
    {
        // Resolve MethodInfo once and compile it into a reusable strongly-typed delegate.
        MethodInfo getter = typeof(SampleEntity).GetProperty(nameof(SampleEntity.Value))!.GetGetMethod()!;
        _getter = (Func<SampleEntity, int>)Delegate.CreateDelegate(typeof(Func<SampleEntity, int>), getter);
    }

    /// <summary>
    /// Gets the <see cref="SampleEntity.Value"/> property via a pre-compiled cached delegate.
    /// </summary>
    /// <param name="entity">The entity to read from.</param>
    /// <returns>The current value of <see cref="SampleEntity.Value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <see langword="null"/>.
    /// </exception>
    public static int GetValueCached(SampleEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _getter(entity);
    }
}
