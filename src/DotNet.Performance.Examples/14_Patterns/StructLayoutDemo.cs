// ============================================================
// Concept  : StructLayout for Memory Packing
// Summary  : Compares Sequential (padded) vs Explicit (manually packed) struct layouts
// When to use   : Interop scenarios, large arrays of structs, or cache-line optimisation
// When NOT to use: Normal business objects where the compiler-default layout is fine
// ============================================================

using System.Runtime.InteropServices;

namespace DotNet.Performance.Examples.Patterns;

/// <summary>
/// A struct with <see cref="LayoutKind.Sequential"/> layout.
/// The runtime inserts padding between <c>A</c> (1 byte) and <c>B</c> (4 bytes) to satisfy
/// alignment requirements, typically resulting in a size of 12 bytes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SequentialPoint
{
    /// <summary>Single-byte field; followed by padding inserted by the runtime.</summary>
    public byte A;

    /// <summary>Four-byte int; aligned to a 4-byte boundary.</summary>
    public int B;

    /// <summary>Single-byte field; followed by trailing padding to keep the struct's size a multiple of 4.</summary>
    public byte C;
}

/// <summary>
/// A struct with <see cref="LayoutKind.Explicit"/> layout.
/// Fields are placed at hand-picked offsets to achieve tighter packing:
/// both byte fields share the first 4-byte word, saving up to 8 bytes compared to
/// <see cref="SequentialPoint"/>.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct ExplicitPoint
{
    /// <summary>Byte at offset 0.</summary>
    [FieldOffset(0)]
    public byte A;

    /// <summary>Byte at offset 1, immediately following <c>A</c>.</summary>
    [FieldOffset(1)]
    public byte C;

    /// <summary>Int at offset 4, aligned to a 4-byte boundary.</summary>
    [FieldOffset(4)]
    public int B;
}

/// <summary>
/// Exposes methods to query the marshalled sizes of <see cref="SequentialPoint"/>
/// and <see cref="ExplicitPoint"/> to compare the impact of automatic vs manual layout.
/// </summary>
public static class StructLayoutDemo
{
    /// <summary>
    /// Returns the marshalled size of <see cref="SequentialPoint"/> in bytes.
    /// Padding inserted by the runtime for alignment typically makes this larger than the sum of field sizes.
    /// </summary>
    /// <returns>Size of <see cref="SequentialPoint"/> as reported by <see cref="Marshal.SizeOf{T}()"/>.</returns>
    public static int GetSequentialSize() => Marshal.SizeOf<SequentialPoint>();

    /// <summary>
    /// Returns the marshalled size of <see cref="ExplicitPoint"/> in bytes.
    /// Manual field offsets allow a more compact layout than <see cref="SequentialPoint"/>.
    /// </summary>
    /// <returns>Size of <see cref="ExplicitPoint"/> as reported by <see cref="Marshal.SizeOf{T}()"/>.</returns>
    public static int GetExplicitSize() => Marshal.SizeOf<ExplicitPoint>();
}
