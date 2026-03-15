// ============================================================
// Concept  : String Interpolation vs string.Format
// Summary  : Compares string.Format (older, boxing-prone) with $ interpolation (compiler-optimized)
// When to use   : Prefer $ interpolation; the compiler emits DefaultInterpolatedStringHandler to minimise allocations
// When NOT to use: Avoid string.Format in hot paths; its object[] params array causes boxing for value types
// ============================================================

namespace DotNet.Performance.Examples.StringOptimization;

/// <summary>
/// Compares <see cref="string.Format(string, object, object)"/> against C# string interpolation (<c>$"…"</c>).
/// Since C# 10, the compiler lowers <c>$"{…}"</c> to an <c>InterpolatedStringHandler</c> that
/// avoids allocating a <c>params object[]</c> and boxes value-type arguments.
/// </summary>
public static class StringInterpolationDemo
{
    /// <summary>
    /// Formats <paramref name="name"/> and <paramref name="count"/> using
    /// <see cref="string.Format(string, object?, object?)"/>.
    /// The <c>int</c> argument is boxed into an <c>object</c> on every call.
    /// </summary>
    /// <param name="name">Name component of the formatted string.</param>
    /// <param name="count">Numeric component of the formatted string.</param>
    /// <returns>A string of the form <c>"name count"</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
    public static string Naive(string name, int count)
    {
        ArgumentNullException.ThrowIfNull(name);

        return string.Format("{0} {1}", name, count);
    }

    /// <summary>
    /// Formats <paramref name="name"/> and <paramref name="count"/> using a C# interpolated string.
    /// The compiler emits an <c>InterpolatedStringHandler</c> that writes directly into a
    /// pre-allocated buffer, avoiding the boxing and the intermediate <c>object[]</c>.
    /// </summary>
    /// <param name="name">Name component of the formatted string.</param>
    /// <param name="count">Numeric component of the formatted string.</param>
    /// <returns>A string of the form <c>"name count"</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
    public static string Optimized(string name, int count)
    {
        ArgumentNullException.ThrowIfNull(name);

        return $"{name} {count}";
    }
}
