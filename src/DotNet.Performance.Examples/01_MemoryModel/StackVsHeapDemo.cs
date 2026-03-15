// ============================================================
// Concept  : Memory Model
// Summary  : Demonstrates value types on the stack vs. reference types on the heap
// When to use   : Understanding memory layout and choosing between structs and classes
// When NOT to use: Structs should not be used for large data types that require frequent copying
// ============================================================

namespace DotNet.Performance.Examples.MemoryModel;

/// <summary>
/// A stack-allocated, immutable point with X and Y fields (value type).
/// Because it is a <c>readonly struct</c>, copies are safe and the compiler can optimise away defensive copies.
/// </summary>
public readonly struct StackPoint
{
    /// <summary>Gets the X coordinate.</summary>
    public int X { get; init; }

    /// <summary>Gets the Y coordinate.</summary>
    public int Y { get; init; }

    /// <summary>Initialises a new <see cref="StackPoint"/>.</summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public StackPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

/// <summary>
/// A heap-allocated point with mutable X and Y properties (reference type).
/// Every instance is managed by the garbage collector.
/// </summary>
public sealed class HeapPoint
{
    /// <summary>Gets or sets the X coordinate.</summary>
    public int X { get; set; }

    /// <summary>Gets or sets the Y coordinate.</summary>
    public int Y { get; set; }

    /// <summary>Initialises a new <see cref="HeapPoint"/>.</summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public HeapPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

/// <summary>
/// Demonstrates the difference between stack allocation (value types) and heap allocation
/// (reference types), including their respective GC impact.
/// </summary>
/// <example>
/// <code>
/// var demo = new StackVsHeapDemo();
/// int stackSum = demo.DemonstrateStackAllocation(); // no heap allocation
/// HeapPoint pt  = demo.DemonstrateHeapAllocation(); // GC-tracked object
/// </code>
/// </example>
public sealed class StackVsHeapDemo
{
    /// <summary>
    /// Allocates three <see cref="StackPoint"/> value types on the stack and returns their coordinate sum.
    /// No heap allocation occurs — the structs live on the current call-stack frame.
    /// </summary>
    /// <returns>The sum of all X and Y fields across the three points (expected: 21).</returns>
    public int DemonstrateStackAllocation()
    {
        // These structs are allocated on the stack; they are freed automatically when the method returns.
        StackPoint a = new(1, 2);
        StackPoint b = new(3, 4);
        StackPoint c = new(5, 6);

        return a.X + a.Y + b.X + b.Y + c.X + c.Y;
    }

    /// <summary>
    /// Allocates a <see cref="HeapPoint"/> on the managed heap and returns it.
    /// The object will be tracked and eventually collected by the garbage collector.
    /// </summary>
    /// <returns>A newly allocated <see cref="HeapPoint"/> with X = 10, Y = 20.</returns>
    public HeapPoint DemonstrateHeapAllocation()
    {
        // The object is promoted to the heap; GC is responsible for its lifetime.
        return new HeapPoint(10, 20);
    }

    /// <summary>
    /// Returns the total number of bytes allocated on the managed heap by the current thread
    /// since the process started, using a precise measurement.
    /// </summary>
    /// <returns>Total allocated bytes as reported by <see cref="GC.GetTotalAllocatedBytes"/>.</returns>
    public long GetTotalAllocatedBytes()
    {
        return GC.GetTotalAllocatedBytes(precise: true);
    }
}
