// ============================================================
// Concept  : Activator.CreateInstance vs new T()
// Summary  : Shows the cost of reflective construction versus constrained generic instantiation
// When to use   : Use Activator only when the type is not known at compile time; prefer new T() otherwise
// When NOT to use: Activator in hot paths — it is significantly slower and prevents JIT inlining
// ============================================================

namespace DotNet.Performance.Examples.Reflection;

/// <summary>
/// Compares creating an instance of <see cref="SampleEntity"/> via
/// <see cref="Activator.CreateInstance{T}"/> (reflective, slower) versus the
/// <c>new T()</c> generic constraint (direct, inlineable by the JIT).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Activator.CreateInstance{T}"/> is implemented via reflection under the hood.
/// It resolves the constructor at runtime, which prevents the JIT from inlining the allocation
/// and adds metadata lookup overhead on every call.
/// </para>
/// <para>
/// <see cref="CreateViaNegativeNew{T}"/> uses the <c>new()</c> generic constraint, allowing
/// the JIT to emit a direct <c>newobj</c> instruction — equivalent to a hard-coded <c>new SampleEntity()</c>
/// at the call site.
/// </para>
/// </remarks>
public static class ActivatorVsNewDemo
{
    /// <summary>
    /// Creates a new <see cref="SampleEntity"/> instance via <see cref="Activator.CreateInstance{T}"/>.
    /// </summary>
    /// <returns>A newly created <see cref="SampleEntity"/> with default property values.</returns>
    public static SampleEntity CreateViaActivator() => Activator.CreateInstance<SampleEntity>();

    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> using the <c>new()</c> generic constraint.
    /// The JIT can inline and devirtualise this call, making it equivalent to a direct constructor call.
    /// </summary>
    /// <typeparam name="T">Any type with a public parameterless constructor.</typeparam>
    /// <returns>A new default instance of <typeparamref name="T"/>.</returns>
    public static T CreateViaNegativeNew<T>() where T : new() => new T();
}
