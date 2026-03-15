// ============================================================
// Concept  : Boxing & Unboxing
// Summary  : Demonstrates GC pressure caused by boxing when using non-generic ArrayList
// When to use   : Understanding why generic collections must be preferred over ArrayList
// When NOT to use: Never use ArrayList in new code — always use List<T>
// ============================================================

using System.Collections;

namespace DotNet.Performance.Examples.BoxingUnboxing;

/// <summary>
/// Compares a non-generic <see cref="ArrayList"/> (which boxes every stored value type)
/// with the generic <see cref="List{T}"/> (which stores value types without boxing).
/// </summary>
/// <example>
/// <code>
/// var demo = new BoxingDemo();
/// int sumNaive      = demo.Naive(1000);      // each int is boxed → object
/// int sumOptimized  = demo.Optimized(1000);  // no boxing; int stored directly
/// </code>
/// </example>
public sealed class BoxingDemo
{
    /// <summary>
    /// Uses <see cref="ArrayList"/> to store integers, boxing each <see cref="int"/> to
    /// <see cref="object"/> on insertion and unboxing it again on retrieval.
    /// This causes one heap allocation per element and adds GC pressure.
    /// </summary>
    /// <param name="count">Number of integers to add and sum. Must be positive.</param>
    /// <returns>The sum of integers 0 through <paramref name="count"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is zero or negative.
    /// </exception>
    public int Naive(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        // ArrayList stores object references — each int is boxed on Add() and unboxed on cast.
        ArrayList list = new(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(i); // boxing: int → object (heap allocation per element)
        }

        int sum = 0;

        foreach (object item in list)
        {
            sum += (int)item; // unboxing: object → int
        }

        return sum;
    }

    /// <summary>
    /// Uses <see cref="List{T}"/> (generic), which stores <see cref="int"/> values directly
    /// on the internal array without any boxing or heap allocation per element.
    /// </summary>
    /// <param name="count">Number of integers to add and sum. Must be positive.</param>
    /// <returns>The sum of integers 0 through <paramref name="count"/> − 1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is zero or negative.
    /// </exception>
    public int Optimized(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        // List<int> stores integers directly in an int[] array — no boxing occurs.
        List<int> list = new(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(i);
        }

        int sum = 0;

        foreach (int item in list)
        {
            sum += item;
        }

        return sum;
    }
}
