// ============================================================
// Concept  : String Concatenation
// Summary  : Contrasts repeated += string concatenation (O(n²) allocations) with StringBuilder
// When to use   : Use StringBuilder whenever combining more than ~3–5 string segments in a loop
// When NOT to use: String + or interpolation is fine for a small, fixed number of literals
// ============================================================

using System.Text;

namespace DotNet.Performance.Examples.StringOptimization;

/// <summary>
/// Demonstrates the performance difference between naive string concatenation
/// (repeated <c>+=</c>) and the allocation-efficient <see cref="StringBuilder"/> approach.
/// </summary>
public static class StringConcatDemo
{
    /// <summary>
    /// Builds a string by concatenating <c>"Hello"</c> <paramref name="count"/> times
    /// using the <c>+=</c> operator.
    /// Each iteration allocates a new intermediate string, resulting in O(n²) total allocations.
    /// </summary>
    /// <param name="count">Number of times to append <c>"Hello"</c>.</param>
    /// <returns>A string consisting of <paramref name="count"/> repetitions of <c>"Hello"</c>.</returns>
    public static string Naive(int count)
    {
        string result = "";
        for (int i = 0; i < count; i++)
        {
            result += "Hello";
        }
        return result;
    }

    /// <summary>
    /// Builds a string by appending <c>"Hello"</c> <paramref name="count"/> times
    /// using <see cref="StringBuilder"/>.
    /// Only one final string allocation occurs when <see cref="StringBuilder.ToString()"/> is called.
    /// </summary>
    /// <param name="count">Number of times to append <c>"Hello"</c>.</param>
    /// <returns>A string consisting of <paramref name="count"/> repetitions of <c>"Hello"</c>.</returns>
    public static string Optimized(int count)
    {
        var sb = new StringBuilder(count * 5);
        for (int i = 0; i < count; i++)
        {
            sb.Append("Hello");
        }
        return sb.ToString();
    }
}
