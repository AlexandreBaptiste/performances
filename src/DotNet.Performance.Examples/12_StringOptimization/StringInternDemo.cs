// ============================================================
// Concept  : String Interning
// Summary  : Demonstrates string.Intern / string.IsInterned to reuse identical string instances
// When to use   : Large sets of repeated, long-lived string values (e.g. parsed identifiers)
// When NOT to use: Short-lived strings or strings unique per request — interning holds them forever
// ============================================================

namespace DotNet.Performance.Examples.StringOptimization;

/// <summary>
/// Demonstrates the .NET string intern pool via <see cref="string.Intern"/> and
/// <see cref="string.IsInterned"/>.
/// Interning ensures that only one object exists for each unique string value,
/// enabling reference-equality comparisons and reducing memory for repeated strings.
/// </summary>
public static class StringInternDemo
{
    /// <summary>
    /// Interns <paramref name="value"/> into the CLR string intern pool.
    /// If an equal string already exists in the pool, that existing instance is returned;
    /// otherwise <paramref name="value"/> is added and then returned.
    /// </summary>
    /// <param name="value">The string to intern.</param>
    /// <returns>The interned string instance from the pool.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string InternString(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.Intern(value);
    }

    /// <summary>
    /// Checks whether <paramref name="value"/> is already present in the CLR string intern pool
    /// without adding it.
    /// </summary>
    /// <param name="value">The string to look up.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is interned; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static bool IsInterned(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.IsInterned(value) is not null;
    }
}
