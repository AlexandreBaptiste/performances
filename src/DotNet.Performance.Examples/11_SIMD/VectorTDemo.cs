// ============================================================
// Concept  : Vector<T> SIMD Vectorization
// Summary  : Demonstrates SIMD-accelerated summation using System.Numerics.Vector<T>
// When to use   : Numerical loops over large arrays where hardware SIMD is available
// When NOT to use: Small arrays where the overhead of vector setup exceeds the gain
// ============================================================

using System.Numerics;

namespace DotNet.Performance.Examples.SIMD;

/// <summary>
/// Compares a scalar loop sum against a SIMD-accelerated sum using <see cref="Vector{T}"/>.
/// <see cref="Vector{T}"/> automatically adapts to the widest vector registers available
/// (SSE2, AVX2, AVX-512) via <see cref="Vector.IsHardwareAccelerated"/>.
/// </summary>
public static class VectorTDemo
{
    /// <summary>
    /// Computes the sum of all elements in <paramref name="data"/> using a plain scalar loop.
    /// </summary>
    /// <param name="data">Input array of integers.</param>
    /// <returns>Sum of all elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long ScalarSum(int[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        long sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }
        return sum;
    }

    /// <summary>
    /// Computes the sum of all elements in <paramref name="data"/> using SIMD via <see cref="Vector{T}"/>.
    /// Processes <see cref="Vector{T}.Count"/> elements per iteration, then handles the remainder
    /// with a scalar loop.
    /// </summary>
    /// <param name="data">Input array of integers.</param>
    /// <returns>Sum of all elements; identical to <see cref="ScalarSum"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public static long VectorizedSum(int[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        int vectorSize = Vector<int>.Count;
        Vector<int> vectorSum = Vector<int>.Zero;
        int i = 0;

        for (; i <= data.Length - vectorSize; i += vectorSize)
        {
            Vector<int> v = new Vector<int>(data, i);
            vectorSum += v;
        }

        long sum = 0;
        for (int j = 0; j < vectorSize; j++)
        {
            sum += vectorSum[j];
        }

        // Scalar remainder
        for (; i < data.Length; i++)
        {
            sum += data[i];
        }

        return sum;
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="Vector{T}"/> operations are accelerated
    /// by hardware SIMD on the current processor.
    /// </summary>
    public static bool IsHardwareAccelerated => Vector.IsHardwareAccelerated;
}
