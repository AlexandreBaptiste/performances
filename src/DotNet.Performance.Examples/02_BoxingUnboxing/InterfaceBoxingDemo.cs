// ============================================================
// Concept  : Boxing & Unboxing
// Summary  : Shows the subtle boxing that occurs when a struct is accessed via a non-generic interface
// When to use   : Understanding interface dispatch cost on value types in hot paths
// When NOT to use: Avoid calling non-generic interfaces on structs in performance-critical code;
//                  use generic constraints or direct struct calls instead
// ============================================================

namespace DotNet.Performance.Examples.BoxingUnboxing;

/// <summary>
/// A non-generic interface used to demonstrate struct boxing via interface dispatch.
/// Assigning a value type to a variable of this interface type causes a heap allocation.
/// </summary>
public interface IValue
{
    /// <summary>Returns the wrapped integer value.</summary>
    int GetValue();
}

/// <summary>
/// A value type (struct) that implements <see cref="IValue"/>.
/// While the struct itself is stack-allocated, assigning it to an <see cref="IValue"/>
/// variable boxes it onto the heap.
/// </summary>
public readonly struct ValueWrapper : IValue
{
    private readonly int _value;

    /// <summary>Initialises a new <see cref="ValueWrapper"/>.</summary>
    /// <param name="value">The integer value to wrap.</param>
    public ValueWrapper(int value)
    {
        _value = value;
    }

    /// <inheritdoc/>
    public int GetValue() => _value;
}

/// <summary>
/// Demonstrates the boxing overhead introduced by non-generic interface dispatch on structs,
/// and how generic type constraints allow the JIT to specialise the call and eliminate boxing.
/// </summary>
/// <example>
/// <code>
/// var demo = new InterfaceBoxingDemo();
/// int sumNaive     = demo.Naive(1000);      // each ValueWrapper is boxed
/// int sumOptimized = demo.Optimized(1000);  // no boxing; JIT specialises the call
/// </code>
/// </example>
public sealed class InterfaceBoxingDemo
{
    /// <summary>
    /// Accumulates values by calling <see cref="IValue.GetValue"/> through an interface reference.
    /// Each struct assigned to <c>IValue wrapper</c> is boxed onto the heap.
    /// </summary>
    /// <param name="count">Number of values to accumulate. Must be positive.</param>
    /// <returns>The sum of values 0 through <paramref name="count"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is zero or negative.
    /// </exception>
    public int Naive(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int sum = 0;

        for (int i = 0; i < count; i++)
        {
            // Assigning a struct to an interface variable causes boxing (one heap alloc per iteration).
            IValue wrapper = new ValueWrapper(i);
            sum += wrapper.GetValue();
        }

        return sum;
    }

    /// <summary>
    /// Accumulates values using a generic helper method constrained to
    /// <c>where T : struct, IValue</c>, which lets the JIT generate a specialised
    /// version without boxing.
    /// </summary>
    /// <param name="count">Number of values to accumulate. Must be positive.</param>
    /// <returns>The sum of values 0 through <paramref name="count"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is zero or negative.
    /// </exception>
    public int Optimized(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int sum = 0;

        for (int i = 0; i < count; i++)
        {
            // The JIT specialises GetValueGeneric<ValueWrapper>; no boxing occurs.
            sum += GetValueGeneric(new ValueWrapper(i));
        }

        return sum;
    }

    // Generic method with struct + IValue constraint.
    // The JIT emits a monomorphised version for ValueWrapper, calling GetValue() directly.
    private static int GetValueGeneric<T>(T wrapper) where T : struct, IValue
    {
        return wrapper.GetValue();
    }
}
