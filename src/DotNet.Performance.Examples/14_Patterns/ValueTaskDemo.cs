// ============================================================
// Concept  : ValueTask<T> vs Task<T>
// Summary  : Shows how ValueTask<T> avoids Task allocation on the synchronous (cached) code path
// When to use   : Methods that frequently return synchronously (e.g. cache hits) to eliminate Task heap allocation
// When NOT to use: Methods that are almost always truly async — the extra struct overhead is not worth it
// ============================================================

namespace DotNet.Performance.Examples.Patterns;

/// <summary>
/// Demonstrates the allocation difference between <see cref="ValueTask{T}"/> and
/// <see cref="Task{T}"/> for a method that returns a cached value synchronously.
/// <para>
/// <see cref="ValueTask{T}"/> can be returned without any heap allocation when the result
/// is already available (cache hit), because the value is embedded directly in the struct.
/// <see cref="Task{T}"/> always allocates a new <see cref="Task{T}"/> object, even for
/// synchronous completion.
/// </para>
/// </summary>
public sealed class ValueTaskDemo
{
    private readonly Dictionary<int, int> _cache = new();

    /// <summary>
    /// Returns the computed value for <paramref name="key"/> as a <see cref="ValueTask{T}"/>.
    /// On a cache hit the value is returned synchronously with <strong>zero allocations</strong>.
    /// On a cache miss the value is computed, cached, and returned via
    /// <see cref="Task.FromResult{T}"/> (one allocation, same as <see cref="GetValueWithTaskAsync"/>).
    /// </summary>
    /// <param name="key">The key to look up or compute.</param>
    /// <returns>A <see cref="ValueTask{T}"/> yielding the computed value.</returns>
    public ValueTask<int> GetCachedValueAsync(int key)
    {
        if (_cache.TryGetValue(key, out int cached))
        {
            return new ValueTask<int>(cached); // Zero allocation — result lives in the struct
        }

        int value = ComputeValue(key);
        _cache[key] = value;
        return new ValueTask<int>(Task.FromResult(value));
    }

    /// <summary>
    /// Returns the computed value for <paramref name="key"/> as a <see cref="Task{T}"/>.
    /// Even on a cache hit, <see cref="Task.FromResult{T}"/> allocates a new <see cref="Task{T}"/>
    /// object, making this pattern less efficient than <see cref="GetCachedValueAsync"/> on hot paths.
    /// </summary>
    /// <param name="key">The key to look up or compute.</param>
    /// <returns>A <see cref="Task{T}"/> yielding the computed value.</returns>
    public Task<int> GetValueWithTaskAsync(int key)
    {
        if (_cache.TryGetValue(key, out int cached))
        {
            return Task.FromResult(cached); // Always allocates a Task object
        }

        int value = ComputeValue(key);
        _cache[key] = value;
        return Task.FromResult(value);
    }

    private static int ComputeValue(int key) => key * key;
}
