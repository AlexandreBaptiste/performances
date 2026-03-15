// ============================================================
// Concept  : Source generator as reflection alternative
// Summary  : Compile-time code generation eliminates runtime reflection with zero overhead and full AOT support
// When to use   : Any scenario where reflection substitutes for unknown compile-time code — serialisation, mapping, DI
// When NOT to use: When the source types are truly unknown at compile time and must be discovered at runtime
// ============================================================

namespace DotNet.Performance.Examples.Reflection;

/// <summary>
/// Educational stub illustrating how Roslyn source generators replace runtime reflection.
/// </summary>
/// <remarks>
/// <para>
/// Source generators are Roslyn components that run at compile time and emit C# code into the
/// compilation. The emitted code is strongly typed, can be inlined by the JIT, and is fully
/// compatible with IL trimming and NativeAOT publishing.
/// </para>
/// <para>
/// Well-known examples in the .NET ecosystem:
/// <list type="bullet">
///   <item>
///     <description>
///       <c>System.Text.Json</c> source generation — add <c>[JsonSerializable(typeof(MyType))]</c>
///       to a <c>partial JsonSerializerContext</c> to get allocation-free, trim-safe serialisation.
///     </description>
///   </item>
///   <item>
///     <description>
///       <a href="https://github.com/riok/mapperly">Mapperly</a> — compile-time object mapper that
///       generates direct property assignments with no runtime reflection.
///     </description>
///   </item>
///   <item>
///     <description>
///       Custom incremental generators — write a Roslyn <c>IIncrementalGenerator</c> to emit
///       property accessors, validators, or DI registrations at build time.
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// To measure the impact: a source-generated property accessor has the same performance profile as
/// hand-written code, while a reflection-based one typically runs 10–100× slower due to boxing,
/// metadata lookup, and invocation overhead.
/// </para>
/// </remarks>
public static class SourceGeneratorNoteDemo
{
    /// <summary>
    /// Returns a human-readable description of the source generator approach
    /// and its advantages over runtime reflection.
    /// </summary>
    /// <returns>A description string.</returns>
    public static string GetDescription() =>
        "Source generators emit code at compile-time, eliminating runtime reflection overhead entirely. " +
        "Use System.Text.Json source generation, AutoMapper AOT, or custom Roslyn generators " +
        "to get zero-cost property access without compromising trimming and AOT compatibility.";
}
