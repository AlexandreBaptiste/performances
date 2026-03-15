// ============================================================
// Concept  : MethodImpl(NoInlining) for visible stack frames
// Summary  : Prevents the JIT from inlining a method so it appears in profiler stack traces
// When to use   : Validation helpers, error-path methods, or any method that must show in crash dumps/profiler
// When NOT to use: Hot inner loops where call overhead matters
// ============================================================

using System.Runtime.CompilerServices;

namespace DotNet.Performance.Examples.Inlining;

/// <summary>
/// Demonstrates using <see cref="MethodImplOptions.NoInlining"/> to keep a method frame
/// visible in profiler stack traces and exception call stacks.
/// </summary>
public static class NoInliningDemo
{
    /// <summary>
    /// Validates that <paramref name="input"/> is neither <see langword="null"/> nor empty.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns><see langword="true"/> when validation passes.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="input"/> is <see langword="null"/> or empty.
    /// </exception>
    /// <remarks>
    /// Decorated with <see cref="MethodImplOptions.NoInlining"/> so that this frame always
    /// appears in profiler traces and exception stack dumps, making it easy to diagnose
    /// where invalid input entered the system.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool ValidateInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input must not be null or empty.", nameof(input));
        }

        return true;
    }

    /// <summary>
    /// Validates <paramref name="input"/> via <see cref="ValidateInput"/> and returns its length.
    /// </summary>
    /// <param name="input">The string to process.</param>
    /// <returns>The length of <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="input"/> is <see langword="null"/> or empty.
    /// </exception>
    public static int ProcessInput(string? input)
    {
        ValidateInput(input);
        return input!.Length;
    }
}
