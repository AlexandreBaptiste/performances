// ============================================================
// Concept  : C# 11 ref fields in ref struct
// Summary  : Demonstrates ref fields that store a managed reference to a variable, enabling in-place mutation
// When to use   : When a stack-only wrapper must hold and mutate a reference to an existing variable without copying
// When NOT to use: When the referenced variable might outlive the ref struct or when heap storage is needed
// ============================================================

namespace DotNet.Performance.Examples.RefStructs;

/// <summary>
/// A <c>ref struct</c> that holds a C# 11 <c>ref</c> field pointing directly to a variable.
/// </summary>
/// <typeparam name="T">The type of the referenced value.</typeparam>
/// <remarks>
/// The <c>ref</c> field stores a managed reference (not a copy) to a variable.
/// Mutation through <see cref="Value"/> affects the original variable directly.
/// All stack-only constraints of a <c>ref struct</c> apply.
/// </remarks>
public ref struct RefCell<T>
{
    /// <summary>A managed reference to the underlying variable.</summary>
    public ref T Value;
}

/// <summary>
/// Demonstrates creating a <see cref="RefCell{T}"/> and mutating the original variable through it.
/// </summary>
public static class RefFieldsDemo
{
    /// <summary>
    /// Creates a <see cref="RefCell{T}"/> that references <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The integer variable to reference.</param>
    /// <returns>A <see cref="RefCell{T}"/> whose <see cref="RefCell{T}.Value"/> points to <paramref name="value"/>.</returns>
    public static RefCell<int> CreateRefCell(ref int value)
    {
        return new RefCell<int> { Value = ref value };
    }

    /// <summary>
    /// Increments the integer referenced by <paramref name="cell"/> in place.
    /// </summary>
    /// <param name="cell">The cell whose referenced value should be incremented.</param>
    public static void Increment(ref RefCell<int> cell)
    {
        cell.Value++;
    }
}
