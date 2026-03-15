// ============================================================
// Concept  : Runtime reflection via PropertyInfo
// Summary  : Reads and writes properties via System.Reflection.PropertyInfo — flexible but slow
// When to use   : One-off tooling, serialisers without AOT requirements, low-frequency generic code
// When NOT to use: Hot paths — reflection allocates metadata descriptors and defeats inlining; use compiled delegates or source generators instead
// ============================================================

using System.Reflection;

namespace DotNet.Performance.Examples.Reflection;

/// <summary>A simple entity used across all Reflection demo classes.</summary>
public class SampleEntity
{
    /// <summary>Gets or sets the numeric value.</summary>
    public int Value { get; set; }

    /// <summary>Gets or sets the entity name.</summary>
    public string Name { get; set; } = "";
}

/// <summary>
/// Demonstrates reading and writing a property via <see cref="PropertyInfo"/> — the naive reflection path.
/// </summary>
/// <remarks>
/// <para>
/// Every call to <see cref="GetValueViaReflection"/> performs a metadata lookup, boxes the
/// <see cref="int"/> result to <see cref="object"/>, and then unboxes it on return.
/// Every call to <see cref="SetValueViaReflection"/> also boxes the value argument on entry.
/// </para>
/// <para>
/// This is typically two to three orders of magnitude slower than a direct property access.
/// Prefer cached delegates (see <c>CachedDelegateDemo</c>) or expression trees
/// (see <c>ExpressionTreeDemo</c>) for any code that runs more than occasionally.
/// </para>
/// </remarks>
public static class ReflectionNaiveDemo
{
    /// <summary>
    /// Gets the <see cref="SampleEntity.Value"/> property via <c>PropertyInfo.GetValue</c>.
    /// </summary>
    /// <param name="entity">The entity to read from.</param>
    /// <returns>The current value of <see cref="SampleEntity.Value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <see langword="null"/>.
    /// </exception>
    public static int GetValueViaReflection(SampleEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // GetValue boxes the int to object; the cast unboxes it — two allocations per call.
        return (int)typeof(SampleEntity).GetProperty(nameof(SampleEntity.Value))!.GetValue(entity)!;
    }

    /// <summary>
    /// Sets the <see cref="SampleEntity.Value"/> property via <c>PropertyInfo.SetValue</c>.
    /// </summary>
    /// <param name="entity">The entity to write to.</param>
    /// <param name="value">The new integer value.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <see langword="null"/>.
    /// </exception>
    public static void SetValueViaReflection(SampleEntity entity, int value)
    {
        ArgumentNullException.ThrowIfNull(entity);

        typeof(SampleEntity).GetProperty(nameof(SampleEntity.Value))!.SetValue(entity, value);
    }
}
