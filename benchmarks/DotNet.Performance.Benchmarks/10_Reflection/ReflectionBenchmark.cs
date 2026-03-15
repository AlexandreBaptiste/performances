using BenchmarkDotNet.Attributes;
using DotNet.Performance.Examples.Reflection;

namespace DotNet.Performance.Benchmarks.Reflection;

/// <summary>
/// Compares three approaches to reading the <c>Value</c> property of <see cref="SampleEntity"/>:
/// raw <see cref="System.Reflection.PropertyInfo"/> (slowest), a cached <see cref="Delegate"/>
/// (fast), and a compiled expression tree (fast).
/// </summary>
/// <remarks>
/// Expected ranking from slowest to fastest: ViaReflection → ViaCachedDelegate ≈ ViaExpressionTree.
/// The cached approaches avoid per-call metadata lookup and boxing.
/// </remarks>
[MemoryDiagnoser]
[RankColumn]
public class ReflectionBenchmark
{
    private static readonly SampleEntity _entity = new() { Value = 42, Name = "benchmark" };

    /// <summary>
    /// Baseline: reads the property via <see cref="System.Reflection.PropertyInfo.GetValue"/>.
    /// Incurs metadata lookup and boxing on every iteration.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int ViaReflection() => ReflectionNaiveDemo.GetValueViaReflection(_entity);

    /// <summary>
    /// Reads the property via a <see cref="Delegate"/> compiled once from <see cref="System.Reflection.MethodInfo"/>.
    /// Near-native speed after the one-time initialisation at class load.
    /// </summary>
    [Benchmark]
    public int ViaCachedDelegate() => CachedDelegateDemo.GetValueCached(_entity);

    /// <summary>
    /// Reads the property via a delegate compiled once from an expression tree.
    /// Near-native speed; the JIT can inline the emitted IL.
    /// </summary>
    [Benchmark]
    public int ViaExpressionTree() => ExpressionTreeDemo.GetValueViaExpression(_entity);
}
