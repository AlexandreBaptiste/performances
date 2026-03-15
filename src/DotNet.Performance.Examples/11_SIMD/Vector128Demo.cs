// ============================================================
// Concept  : Vector128<T> Fixed-Width SIMD
// Summary  : Demonstrates 128-bit SIMD dot product using System.Runtime.Intrinsics.Vector128<float>
// When to use   : When you need predictable 128-bit vector width regardless of platform
// When NOT to use: When portability to non-SIMD targets is unknown — always check IsHardwareAccelerated
// ============================================================

using System.Runtime.Intrinsics;

namespace DotNet.Performance.Examples.SIMD;

/// <summary>
/// Compares a scalar dot product against a 128-bit SIMD dot product using <see cref="Vector128{T}"/>.
/// Unlike <see cref="System.Numerics.Vector{T}"/>, <see cref="Vector128{T}"/> has a fixed width of
/// 128 bits (4 floats), making its behaviour completely predictable across all platforms.
/// </summary>
public static class Vector128Demo
{
    /// <summary>
    /// Computes the dot product of two float arrays using a scalar loop.
    /// </summary>
    /// <param name="a">First input array.</param>
    /// <param name="b">Second input array. Length must be ≥ <paramref name="a"/>.Length.</param>
    /// <returns>Dot product of <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
    public static float DotProductScalar(float[] a, float[] b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        int length = Math.Min(a.Length, b.Length);
        float result = 0f;
        for (int i = 0; i < length; i++)
        {
            result += a[i] * b[i];
        }
        return result;
    }

    /// <summary>
    /// Computes the dot product of two float arrays using <see cref="Vector128{T}"/>.
    /// Falls back to <see cref="DotProductScalar"/> when hardware acceleration is unavailable
    /// or when the arrays contain fewer than 4 elements.
    /// Processes 4 floats per iteration and handles the remainder with a scalar tail loop.
    /// </summary>
    /// <param name="a">First input array.</param>
    /// <param name="b">Second input array. Length must be ≥ <paramref name="a"/>.Length.</param>
    /// <returns>Dot product of <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
    public static float DotProductVector128(float[] a, float[] b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        int length = Math.Min(a.Length, b.Length);

        if (!Vector128.IsHardwareAccelerated || length < Vector128<float>.Count)
        {
            return DotProductScalar(a, b);
        }

        int vectorLength = Vector128<float>.Count; // 4 floats
        Vector128<float> vectorSum = Vector128<float>.Zero;
        int i = 0;

        for (; i <= length - vectorLength; i += vectorLength)
        {
            Vector128<float> va = Vector128.Create(a.AsSpan(i, vectorLength));
            Vector128<float> vb = Vector128.Create(b.AsSpan(i, vectorLength));
            vectorSum += va * vb;
        }

        float result = Vector128.Sum(vectorSum);

        // Scalar tail for remaining elements
        for (; i < length; i++)
        {
            result += a[i] * b[i];
        }

        return result;
    }
}
