// ============================================================
// Concept  : SkipLocalsInit attribute
// Summary  : Demonstrates suppressing stack-local zero-initialization for hot stackalloc paths
// When to use   : Performance-critical methods with stackalloc where all locals are written before read
// When NOT to use: Any method where uninitialized memory could be read — risk of undefined behaviour
// ============================================================

// UNSAFE — educational only. SkipLocalsInit means uninitialized memory — always write before read.
using System.Runtime.CompilerServices;

[module: SkipLocalsInit]

namespace DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;

/// <summary>
/// Demonstrates the effect of <see cref="SkipLocalsInitAttribute"/> on <c>stackalloc</c> buffers.
/// </summary>
/// <remarks>
/// <para>
/// By default, the C# runtime emits the <c>.locals init</c> IL flag, which instructs the JIT to
/// zero-initialise all local variables (including <c>stackalloc</c> buffers) before the method body runs.
/// </para>
/// <para>
/// The <c>[module: SkipLocalsInit]</c> attribute at the top of this file suppresses that flag for every
/// method in this module, allowing the JIT to skip the zeroing pass and reduce stack setup cost.
/// </para>
/// <para>
/// <strong>Safety rule:</strong> always write every element of a <c>stackalloc</c> buffer before reading
/// it when <see cref="SkipLocalsInitAttribute"/> is in effect — otherwise you read uninitialised memory.
/// </para>
/// </remarks>
public static class SkipLocalsInitDemo
{
    private const int MaxSize = 256;

    /// <summary>
    /// Naive version: explicitly clears the buffer before filling, performing redundant work.
    /// Even under <see cref="SkipLocalsInitAttribute"/>, the manual <c>Clear()</c> re-adds the cost
    /// that the optimised path avoids.
    /// </summary>
    /// <param name="size">Number of bytes to process; must be between 1 and 256 inclusive.</param>
    /// <returns>The sum of the byte sequence 0, 1, …, <paramref name="size"/>-1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is less than 1 or greater than 256.
    /// </exception>
    public static int Naive(int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(size, MaxSize);

        Span<byte> buffer = stackalloc byte[size];
        buffer.Clear(); // Redundant zero-fill — represents the cost normally hidden inside .locals init

        for (int i = 0; i < size; i++)
        {
            buffer[i] = (byte)i;
        }

        int sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += buffer[i];
        }

        return sum;
    }

    /// <summary>
    /// Optimised version: relies on <see cref="SkipLocalsInitAttribute"/> to skip zero-initialisation.
    /// Safe because every element is written before it is read — the result is identical to <see cref="Naive"/>.
    /// </summary>
    /// <param name="size">Number of bytes to process; must be between 1 and 256 inclusive.</param>
    /// <returns>The sum of the byte sequence 0, 1, …, <paramref name="size"/>-1.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is less than 1 or greater than 256.
    /// </exception>
    public static int Optimized(int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(size, MaxSize);

        // No Clear() — [module: SkipLocalsInit] suppresses .locals init, so the JIT skips zeroing.
        // Safe: buffer[i] = (byte)i writes before the corresponding read in the second loop.
        Span<byte> buffer = stackalloc byte[size];

        for (int i = 0; i < size; i++)
        {
            buffer[i] = (byte)i;
        }

        int sum = 0;

        for (int i = 0; i < size; i++)
        {
            sum += buffer[i];
        }

        return sum;
    }
}
