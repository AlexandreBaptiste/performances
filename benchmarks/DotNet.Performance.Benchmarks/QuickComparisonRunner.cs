// ============================================================
// Concept  : Quick Comparison Runner
// Summary  : Stopwatch-based Naive vs Optimized measurement for all 14 topics.
// When to use   : Fast local comparison; for rigorous results use BenchmarkDotNet.
// When NOT to use: CI pipelines or decisions requiring statistical confidence.
// ============================================================
// Usage: dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --quick-compare
// ============================================================

using System.Buffers;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using DotNet.Performance.Examples.ArrayPool;
using DotNet.Performance.Examples.BoxingUnboxing;
using DotNet.Performance.Examples.GarbageCollector;
using DotNet.Performance.Examples.GCRegionsAndPGO;
using DotNet.Performance.Examples.Inlining;
using DotNet.Performance.Examples.MemoryModel;
using DotNet.Performance.Examples.ObjectPooling;
using DotNet.Performance.Examples.Patterns;
using DotNet.Performance.Examples.Reflection;
using DotNet.Performance.Examples.RefStructs;
using DotNet.Performance.Examples.SIMD;
using DotNet.Performance.Examples.SkipLocalsInitAndUnsafe;
using DotNet.Performance.Examples.SpanAndMemory;
using DotNet.Performance.Examples.StringOptimization;

namespace DotNet.Performance.Benchmarks;

/// <summary>
/// Runs every Naive/Optimized pair with <see cref="Stopwatch"/> and prints a comparison table.
/// Results are approximate single-process measurements. Use BenchmarkDotNet for statistically rigorous numbers.
/// </summary>
internal static class QuickComparisonRunner
{
    private const int WarmupIterations = 500;
    private const int MeasureIterations = 5_000;

    /// <summary>Entry point for the quick comparison mode.</summary>
    internal static void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine();
        Console.WriteLine("=================================================================");
        Console.WriteLine(" DotNet.Performance — Quick Comparison (Stopwatch-based)");
        Console.WriteLine($" Iterations : {MeasureIterations:N0} per method");
        Console.WriteLine($" Runtime    : .NET {Environment.Version}  |  {RuntimeInformation()}");
        Console.WriteLine(" Tip        : --filter \"*\" for rigorous BenchmarkDotNet numbers.");
        Console.WriteLine("=================================================================");
        Console.WriteLine();

        var results = new List<ComparisonResult>();

        // ── 01 Memory Model ───────────────────────────────────────────────────
        var alloc = new AllocationPressure();
        results.Add(Measure("01 MemoryModel", "AllocationPressure (size=100)",
            () => alloc.Naive(100), () => alloc.Optimized(100)));
        results.Add(Measure("01 MemoryModel", "AllocationPressure (size=1000)",
            () => alloc.Naive(1000), () => alloc.Optimized(1000)));

        // ── 02 Boxing & Unboxing ─────────────────────────────────────────────
        var boxing = new BoxingDemo();
        results.Add(Measure("02 BoxingUnboxing", "ArrayList vs List<int> (100)",
            () => boxing.Naive(100), () => boxing.Optimized(100)));
        results.Add(Measure("02 BoxingUnboxing", "ArrayList vs List<int> (1000)",
            () => boxing.Naive(1000), () => boxing.Optimized(1000)));

        var ifaceBoxing = new InterfaceBoxingDemo();
        results.Add(Measure("02 BoxingUnboxing", "IValue interface vs generic (1000)",
            () => ifaceBoxing.Naive(1000), () => ifaceBoxing.Optimized(1000)));

        // ── 03 Span<T> & Memory<T> ───────────────────────────────────────────
        var spanDemo = new SpanDemo();
        const string spanInput = "The quick brown fox jumps over the lazy dog";
        results.Add(Measure("03 SpanAndMemory", "Substring vs AsSpan().Slice()",
            () => spanDemo.Naive(spanInput, 4, 5).Length,
            () => spanDemo.Optimized(spanInput, 4, 5).Length));

        var stackDemo = new StackallocDemo();
        results.Add(Measure("03 SpanAndMemory", "new byte[] vs stackalloc (256)",
            () => stackDemo.Naive(256), () => stackDemo.Optimized(256)));

        // ── 04 ArrayPool ─────────────────────────────────────────────────────
        var arrayPool = new ArrayPoolDemo();
        results.Add(Measure("04 ArrayPool", "new byte[] vs ArrayPool.Rent (1000)",
            () => arrayPool.Naive(1000), () => arrayPool.Optimized(1000)));

        var memPool = new MemoryPoolDemo();
        results.Add(Measure("04 ArrayPool", "new byte[] vs MemoryPool.Rent (1000)",
            () => memPool.ProcessWithNew(1000), () => memPool.ProcessWithMemoryPool(1000)));

        // ── 05 ref struct ────────────────────────────────────────────────────
        results.Add(Measure("05 RefStructs", "new int[] vs stackalloc+ref struct (64)",
            () =>
            {
                int[] arr = new int[64];
                for (int j = 0; j < arr.Length; j++) arr[j] = j;
                int s = 0; for (int j = 0; j < arr.Length; j++) s += arr[j];
                return s;
            },
            () =>
            {
                Span<int> scratch = stackalloc int[64];
                for (int j = 0; j < scratch.Length; j++) scratch[j] = j;
                StackBuffer buf = RefStructDemo.CreateStackBuffer(scratch);
                return RefStructDemo.SumBuffer(buf);
            }));

        // ── 06 Garbage Collector ─────────────────────────────────────────────
        results.Add(Measure("06 GarbageCollector", "new byte[1000] vs ArrayPool (GC pressure)",
            () =>
            {
                byte[] buf = new byte[1000];
                long s = 0;
                for (int j = 0; j < 1000; j++) { buf[j] = (byte)(j % 256); s += buf[j]; }
                return s;
            },
            () =>
            {
                byte[] buf = ArrayPool<byte>.Shared.Rent(1000);
                try
                {
                    long s = 0;
                    for (int j = 0; j < 1000; j++) { buf[j] = (byte)(j % 256); s += buf[j]; }
                    return s;
                }
                finally { ArrayPool<byte>.Shared.Return(buf); }
            }));

        results.Add(Measure("06 GarbageCollector", "IDisposable vs Finalizer",
            () => { FinalizerDemo.CreateAndForgetNaive(); return 0; },
            () => { FinalizerDemo.CreateAndDisposeProper(); return 0; }));

        // ── 07 GC Regions & Frozen Collections ───────────────────────────────
        IEnumerable<KeyValuePair<string, int>> dictPairs =
            Enumerable.Range(0, 100).Select(j => new KeyValuePair<string, int>($"key{j}", j));
        Dictionary<string, int> mutableDict = dictPairs.ToDictionary(p => p.Key, p => p.Value);
        FrozenDictionary<string, int> frozenDict = FrozenCollectionsDemo.CreateFrozenDictionary(dictPairs);
        results.Add(Measure("07 GCRegionsAndPGO", "Dictionary vs FrozenDictionary lookup",
            () => FrozenCollectionsDemo.LookupNaive(mutableDict, "key50"),
            () => FrozenCollectionsDemo.LookupFrozen(frozenDict, "key50")));

        // ── 08 Inlining ──────────────────────────────────────────────────────
        results.Add(Measure("08 Inlining", "Default vs AggressiveInlining (x=42)",
            () => AggressiveInliningDemo.Naive(42),
            () => AggressiveInliningDemo.Optimized(42)));

        int[] bigArr = Enumerable.Range(0, 10000).ToArray();
        results.Add(Measure("08 Inlining", "Default vs AggressiveOptimization (10k elems)",
            () => AggressiveOptimizationDemo.NaiveSum(bigArr),
            () => AggressiveOptimizationDemo.OptimizedSum(bigArr)));

        // ── 09 SkipLocalsInit & Unsafe ────────────────────────────────────────
        results.Add(Measure("09 SkipLocalsInit", "Clear+Fill vs SkipLocalsInit+Fill (256)",
            () => SkipLocalsInitDemo.Naive(256),
            () => SkipLocalsInitDemo.Optimized(256)));

        int[] src = Enumerable.Range(0, 100).ToArray();
        int[] dst = new int[100];
        results.Add(Measure("09 SkipLocalsInit", "Manual offset copy vs Unsafe.Add copy",
            () => { int cnt = 0; for (int j = 10; j < src.Length; j++) dst[cnt++] = src[j]; return cnt; },
            () => UnsafeClassDemo.CopyWithOffset(src, dst, 10)));

        // ── 10 Reflection ────────────────────────────────────────────────────
        SampleEntity entity = new() { Value = 42, Name = "test" };
        results.Add(Measure("10 Reflection", "PropertyInfo.GetValue vs cached Func<>",
            () => ReflectionNaiveDemo.GetValueViaReflection(entity),
            () => CachedDelegateDemo.GetValueCached(entity)));
        results.Add(Measure("10 Reflection", "PropertyInfo.GetValue vs ExpressionTree",
            () => ReflectionNaiveDemo.GetValueViaReflection(entity),
            () => ExpressionTreeDemo.GetValueViaExpression(entity)));
        results.Add(Measure("10 Reflection", "Activator.CreateInstance vs new T()",
            () => { ActivatorVsNewDemo.CreateViaActivator(); return 0; },
            () => { ActivatorVsNewDemo.CreateViaNegativeNew<SampleEntity>(); return 0; }));

        // ── 11 SIMD ──────────────────────────────────────────────────────────
        int[] simdData = Enumerable.Range(0, 10000).ToArray();
        results.Add(Measure("11 SIMD", "ScalarSum vs Vector<int> (10000 ints)",
            () => VectorTDemo.ScalarSum(simdData),
            () => VectorTDemo.VectorizedSum(simdData)));

        float[] vecA = Enumerable.Range(0, 100).Select(j => (float)j).ToArray();
        results.Add(Measure("11 SIMD", "DotProduct scalar vs Vector128 (100 floats)",
            () => (long)Vector128Demo.DotProductScalar(vecA, vecA),
            () => (long)Vector128Demo.DotProductVector128(vecA, vecA)));

        // ── 12 String Optimization ───────────────────────────────────────────
        results.Add(Measure("12 StringOpt", "string+= vs StringBuilder (50 segments)",
            () => StringConcatDemo.Naive(50).Length,
            () => StringConcatDemo.Optimized(50).Length));
        results.Add(Measure("12 StringOpt", "string.Format vs interpolation",
            () => StringInterpolationDemo.Naive("Alice", 42).Length,
            () => StringInterpolationDemo.Optimized("Alice", 42).Length));
        results.Add(Measure("12 StringOpt", "IndexOfAny vs SearchValues<char>",
            () => SearchValuesDemo.IndexOfVowelNaive(spanInput),
            () => SearchValuesDemo.IndexOfVowelOptimized(spanInput)));

        // ── 13 Object Pooling ─────────────────────────────────────────────────
        results.Add(Measure("13 ObjectPool", "new StringBuilder vs SimpleObjectPool (100 ops)",
            () => ObjectPoolDemo.ProcessWithNew(100),
            () => ObjectPoolDemo.ProcessWithPool(100)));

        // ── 14 Advanced Patterns ─────────────────────────────────────────────
        List<int> marshalList = Enumerable.Range(0, 10000).ToList();
        results.Add(Measure("14 Patterns", "LINQ Sum vs CollectionsMarshal.AsSpan (10k)",
            () => CollectionsMarshalDemo.SumWithLinq(marshalList),
            () => CollectionsMarshalDemo.SumWithMarshal(marshalList)));

        ValueTaskDemo vtDemo = new();
        results.Add(Measure("14 Patterns", "Task<T> vs ValueTask<T> (sync hot path)",
            () => vtDemo.GetValueWithTaskAsync(1).GetAwaiter().GetResult(),
            () => vtDemo.GetCachedValueAsync(1).GetAwaiter().GetResult()));

        // ── Output ────────────────────────────────────────────────────────────
        PrintTable(results);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ComparisonResult Measure<T>(
        string category, string scenario, Func<T> naive, Func<T> optimized)
    {
        for (int i = 0; i < WarmupIterations; i++) naive();
        for (int i = 0; i < WarmupIterations; i++) optimized();

        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        long naiveTicks = MeasureTicks(naive);
        GC.Collect(0, GCCollectionMode.Forced, blocking: true);
        long optimizedTicks = MeasureTicks(optimized);

        double naiveNs = TicksToNs(naiveTicks);
        double optimizedNs = TicksToNs(optimizedTicks);
        double gainPct = naiveNs > 0 ? (naiveNs - optimizedNs) / naiveNs * 100.0 : 0;

        string gainLabel = gainPct >= 0 ? $"  +{gainPct:F1}%" : $"  {gainPct:F1}%";
        Console.WriteLine($"  [{category}] {scenario}");
        Console.WriteLine($"    Naive={naiveNs,9:F2} ns | Optimized={optimizedNs,9:F2} ns | Gain={gainLabel}");

        return new ComparisonResult(category, scenario, naiveNs, optimizedNs, gainPct);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long MeasureTicks<T>(Func<T> method)
    {
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < MeasureIterations; i++) _ = method();
        sw.Stop();
        return sw.ElapsedTicks;
    }

    private static double TicksToNs(long ticks) =>
        (double)ticks / Stopwatch.Frequency * 1_000_000_000.0 / MeasureIterations;

    private static string RuntimeInformation() =>
        System.Runtime.InteropServices.RuntimeInformation.OSDescription;

    private static void PrintTable(List<ComparisonResult> results)
    {
        const string sep = "══════════════════════════════════════════════════════════════════════════════════════════════════════════════";

        Console.WriteLine();
        Console.WriteLine(sep);
        Console.WriteLine($"  {"#",-3} {"Category",-22} {"Scenario",-48} {"Naive (ns)",11} {"Opt (ns)",11} {"Gain",9}");
        Console.WriteLine(sep);

        string? lastCat = null;
        int idx = 1;
        foreach (ComparisonResult r in results)
        {
            if (lastCat is not null && lastCat != r.Category) Console.WriteLine();
            string gain = r.GainPercent >= 0 ? $"+{r.GainPercent:F1}%" : $"{r.GainPercent:F1}%";
            Console.WriteLine($"  {idx++,-3} {r.Category,-22} {r.Scenario,-48} {r.NaiveNs,11:F2} {r.OptimizedNs,11:F2} {gain,9}");
            lastCat = r.Category;
        }

        Console.WriteLine(sep);
        Console.WriteLine("  Gain: positive = optimized is faster | Single-process Stopwatch estimates.");
        Console.WriteLine();

        string md = BuildMarkdown(results);
        Console.WriteLine(md);

        string outputPath = Path.GetFullPath(
            Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory)!, "../../../../quick-comparison.md"));
        File.WriteAllText(outputPath, md);
        Console.WriteLine($"Markdown saved to: {outputPath}");
    }

    private static string BuildMarkdown(List<ComparisonResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Performance Comparison Results");
        sb.AppendLine();
        sb.AppendLine($"> Generated {DateTime.Now:yyyy-MM-dd HH:mm} · .NET {Environment.Version} · {MeasureIterations:N0} iterations per method (Stopwatch, Release build)");
        sb.AppendLine();
        sb.AppendLine("| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |");
        sb.AppendLine("|:-:|:---------|:---------|:---:|:---:|:---:|");

        int i = 1;
        foreach (ComparisonResult r in results)
        {
            string gain = r.GainPercent >= 0
                ? $"**+{r.GainPercent:F1}%** ✅"
                : $"{r.GainPercent:F1}% ⚠️";
            sb.AppendLine($"| {i++} | {r.Category} | {r.Scenario} | {r.NaiveNs:F2} | {r.OptimizedNs:F2} | {gain} |");
        }

        sb.AppendLine();
        sb.AppendLine("> **Gain** = (Naive − Optimized) / Naive × 100. Positive = optimized is faster.");
        sb.AppendLine("> These are approximate single-process Stopwatch measurements. Run BenchmarkDotNet for rigorous statistics:");
        sb.AppendLine("> ```bash");
        sb.AppendLine("> dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter \"*\"");
        sb.AppendLine("> ```");
        return sb.ToString();
    }

    private sealed record ComparisonResult(
        string Category,
        string Scenario,
        double NaiveNs,
        double OptimizedNs,
        double GainPercent);
}
