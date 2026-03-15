// ============================================================
// Concept  : Expression tree compiled accessor
// Summary  : Compiles an Expression<Func<T,R>> at runtime to obtain a delegate with native call performance
// When to use   : ORM-style mappers, validation frameworks, and serialisers that need flexible yet fast accessors
// When NOT to use: Hot paths in AOT/NativeAOT builds where Expression.Compile is unavailable; prefer source generators
// ============================================================

using System.Linq.Expressions;

namespace DotNet.Performance.Examples.Reflection;

/// <summary>
/// Demonstrates building a property accessor from an expression tree and compiling it once for reuse.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/> followed by
/// <see cref="LambdaExpression.Compile()"/> emits IL at runtime and returns a strongly-typed delegate.
/// The emitted IL is equivalent to what the C# compiler would generate for a direct property access.
/// </para>
/// <para>
/// The compiled delegate is stored in a <c>static readonly</c> field so the compilation cost is paid
/// only once at class initialisation. All subsequent calls to <see cref="GetValueViaExpression"/>
/// operate at near-native speed — no boxing, no metadata lookup.
/// </para>
/// </remarks>
public static class ExpressionTreeDemo
{
    private static readonly Func<SampleEntity, int> _accessor;

    static ExpressionTreeDemo()
    {
        // Build the expression: (SampleEntity e) => e.Value
        ParameterExpression param = Expression.Parameter(typeof(SampleEntity), "e");
        MemberExpression property = Expression.Property(param, nameof(SampleEntity.Value));
        _accessor = Expression.Lambda<Func<SampleEntity, int>>(property, param).Compile();
    }

    /// <summary>
    /// Gets the <see cref="SampleEntity.Value"/> property via a compiled expression tree delegate.
    /// </summary>
    /// <param name="entity">The entity to read from.</param>
    /// <returns>The current value of <see cref="SampleEntity.Value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <see langword="null"/>.
    /// </exception>
    public static int GetValueViaExpression(SampleEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _accessor(entity);
    }
}
