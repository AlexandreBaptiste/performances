// ============================================================
// Concept  : Dynamic PGO (Profile-Guided Optimisation) and tiered compilation
// Summary  : Shows a hot loop that benefits from tiered compilation / Dynamic PGO vs a cold one-time method
// When to use   : CPU-bound methods called thousands of times per second benefit most from PGO
// When NOT to use: One-off initialisation methods rarely justify the JIT overhead of collecting profiles
// ============================================================

namespace DotNet.Performance.Examples.GCRegionsAndPGO;

/// <summary>
/// Illustrates code patterns that benefit (or do not benefit) from Dynamic PGO.
/// </summary>
/// <remarks>
/// Enable Dynamic PGO by setting the environment variable <c>DOTNET_TieredPGO=1</c>
/// (the default in .NET 8 and later). The JIT collects a profile during Tier-0 execution
/// and re-compiles hot methods at Tier-1 with profile-guided optimisations such as
/// de-virtualisation, branch prediction hints, and register allocation improvements.
/// </remarks>
public static class DynamicPGODemo
{
    /// <summary>
    /// A loop-heavy method that computes the sum of squares of integers from 0 to
    /// <paramref name="iterations"/> − 1.
    /// This method is a good candidate for tiered compilation because the JIT can
    /// observe the tight inner loop and apply profile-guided optimisations on re-compilation.
    /// </summary>
    /// <param name="iterations">The number of iterations. Must be non-negative.</param>
    /// <returns>The sum of squares: 0² + 1² + … + (<paramref name="iterations"/> − 1)².</returns>
    public static long HotLoop(int iterations)
    {
        long sum = 0;

        for (int i = 0; i < iterations; i++)
        {
            sum += (long)i * i;
        }

        return sum;
    }

    /// <summary>
    /// A simple one-time-use method that illustrates code that does <em>not</em> benefit from PGO:
    /// it runs only once during application startup so the JIT never accumulates a useful profile.
    /// </summary>
    /// <returns>A constant initialisation value.</returns>
    public static int ColdMethod()
    {
        // One-time initialisation logic — not worth PGO profiling overhead.
        return 42;
    }
}
