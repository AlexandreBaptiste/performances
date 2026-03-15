// ============================================================
// Concept  : ref struct as stack-only value type
// Summary  : Demonstrates ref struct with Span<T> field for zero-allocation stack buffers
// When to use   : When you need a slice or buffer that must stay on the stack and never escape to the heap
// When NOT to use: When the buffer must outlive the current stack frame, be stored in a field, or used in async code
// ============================================================

namespace DotNet.Performance.Examples.RefStructs;

/// <summary>
/// A stack-only buffer wrapping a <see cref="Span{T}"/> of <see cref="int"/>.
/// </summary>
/// <remarks>
/// Because <c>StackBuffer</c> is a <c>ref struct</c>, it carries the following constraints:
/// <list type="bullet">
///   <item><description>Cannot be stored as a field in a class or a non-ref struct.</description></item>
///   <item><description>Cannot be used as a type argument in generics.</description></item>
///   <item><description>Cannot be captured by a lambda or used across <c>await</c> boundaries.</description></item>
///   <item><description>Its lifetime is strictly bounded to the stack frame in which it was created.</description></item>
/// </list>
/// </remarks>
public ref struct StackBuffer
{
    /// <summary>Gets the span of integers managed by this buffer.</summary>
    public Span<int> Data;
}

/// <summary>
/// Demonstrates creating and summing a stack-allocated <see cref="StackBuffer"/>
/// to avoid any heap allocation for temporary integer data.
/// </summary>
public static class RefStructDemo
{
    private const int MaxStackSize = 64;

    /// <summary>
    /// Creates a <see cref="StackBuffer"/> wrapping a caller-supplied <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="data">
    /// The span to wrap — ideally backed by a <c>stackalloc</c> in the caller to avoid heap allocation.
    /// Must not be empty and must not exceed <c>64</c> elements.
    /// </param>
    /// <returns>A <see cref="StackBuffer"/> whose <see cref="StackBuffer.Data"/> refers to <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="data"/> is empty or longer than 64 elements.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The idiomatic C# pattern for zero-allocation buffers is to allocate on the stack at the call site:
    /// <code>
    /// Span&lt;int&gt; scratch = stackalloc int[16];
    /// StackBuffer buf = RefStructDemo.CreateStackBuffer(scratch);
    /// </code>
    /// Putting <c>stackalloc</c> inside a factory method would cause the manufactured <see cref="StackBuffer"/>
    /// to reference a dead stack frame — the C# ref-safety rules forbid returning a span over
    /// stackalloc memory from the allocating method.
    /// </para>
    /// </remarks>
    public static StackBuffer CreateStackBuffer(Span<int> data)
    {
        if (data.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Buffer must not be empty.");
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, MaxStackSize);

        return new StackBuffer { Data = data };
    }

    /// <summary>
    /// Sums all elements in the <see cref="StackBuffer.Data"/> span.
    /// </summary>
    /// <param name="buffer">The buffer to sum.</param>
    /// <returns>The sum of all integer elements.</returns>
    public static int SumBuffer(StackBuffer buffer)
    {
        int sum = 0;

        for (int i = 0; i < buffer.Data.Length; i++)
        {
            sum += buffer.Data[i];
        }

        return sum;
    }
}
