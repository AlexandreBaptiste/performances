## Performance Comparison Results

> Generated 2026-03-15 · .NET 10.0.4 on Windows 10.0.26200 · 5,000 iterations per method (Stopwatch, Release build)

| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |
|:-:|:---------|:---------|:---:|:---:|:---:|
| 1 | 01 MemoryModel | AllocationPressure (size=100) | 1,672.50 | 2,466.04 | -47.4% ⚠️ |
| 2 | 01 MemoryModel | AllocationPressure (size=1000) | 10,991.84 | 8,971.20 | **+18.4%** ✅ |
| 3 | 02 BoxingUnboxing | ArrayList vs List&lt;int&gt; (100) | 7,825.20 | 2,829.68 | **+63.8%** ✅ |
| 4 | 02 BoxingUnboxing | ArrayList vs List&lt;int&gt; (1000) | 35,217.04 | 2,550.36 | **+92.8%** ✅ |
| 5 | 02 BoxingUnboxing | IValue interface vs generic (1000) | 40,049.58 | 941.68 | **+97.6%** ✅ |
| 6 | 03 SpanAndMemory | Substring vs AsSpan().Slice() | 53.48 | 60.18 | -12.5% ⚠️ |
| 7 | 03 SpanAndMemory | new byte[] vs stackalloc (256) | 4,696.32 | 516.60 | **+89.0%** ✅ |
| 8 | 04 ArrayPool | new byte[] vs ArrayPool.Rent (1000) | 10,507.40 | 12,541.38 | -19.4% ⚠️ |
| 9 | 04 ArrayPool | new byte[] vs MemoryPool.Rent (1000) | 15,627.88 | 14,656.66 | **+6.2%** ✅ |
| 10 | 05 RefStructs | new int[] vs stackalloc+ref struct (64) | 716.50 | 422.02 | **+41.1%** ✅ |
| 11 | 06 GarbageCollector | new byte[1000] vs ArrayPool (GC pressure) | 8,128.24 | 5,969.02 | **+26.6%** ✅ |
| 12 | 06 GarbageCollector | IDisposable vs Finalizer | 59.44 | 134.16 | -125.7% ⚠️ |
| 13 | 07 GCRegionsAndPGO | Dictionary vs FrozenDictionary lookup | 35.26 | 84.70 | -140.2% ⚠️ |
| 14 | 08 Inlining | Default vs AggressiveInlining (x=42) | 11.40 | 11.14 | **+2.3%** ✅ |
| 15 | 08 Inlining | Default vs AggressiveOptimization (10k elems) | 26,833.16 | 4,473.84 | **+83.3%** ✅ |
| 16 | 09 SkipLocalsInit | Clear+Fill vs SkipLocalsInit+Fill (256) | 203.28 | 221.78 | -9.1% ⚠️ |
| 17 | 09 SkipLocalsInit | Manual offset copy vs Unsafe.Add copy | 672.76 | 503.38 | **+25.2%** ✅ |
| 18 | 10 Reflection | PropertyInfo.GetValue vs cached Func&lt;&gt; | 105.14 | 16.10 | **+84.7%** ✅ |
| 19 | 10 Reflection | PropertyInfo.GetValue vs ExpressionTree | 130.18 | 15.46 | **+88.1%** ✅ |
| 20 | 10 Reflection | Activator.CreateInstance vs new T() | 52.22 | 44.28 | **+15.2%** ✅ |
| 21 | 11 SIMD | ScalarSum vs Vector&lt;int&gt; (10000 ints) | 33,445.24 | 16,065.50 | **+52.0%** ✅ |
| 22 | 11 SIMD | DotProduct scalar vs Vector128 (100 floats) | 796.90 | 2,030.00 | -154.7% ⚠️ |
| 23 | 12 StringOpt | string+= vs StringBuilder (50 segments) | 3,285.32 | 1,313.08 | **+60.0%** ✅ |
| 24 | 12 StringOpt | string.Format vs interpolation | 117.90 | 90.54 | **+23.2%** ✅ |
| 25 | 12 StringOpt | IndexOfAny vs SearchValues&lt;char&gt; | 275.20 | 114.40 | **+58.4%** ✅ |
| 26 | 13 ObjectPool | new StringBuilder vs SimpleObjectPool (100 ops) | 5,769.28 | 18,747.02 | -224.9% ⚠️ |
| 27 | 14 Patterns | LINQ Sum vs CollectionsMarshal.AsSpan (10k) | 45,125.14 | 5,230.38 | **+88.4%** ✅ |
| 28 | 14 Patterns | Task&lt;T&gt; vs ValueTask&lt;T&gt; (sync hot path) | 77.40 | 1,659.30 | -2043.8% ⚠️ |

> **Gain** = (Naive − Optimized) / Naive × 100. Positive = optimized is faster.

---

## Notes on ⚠️ Results

Several scenarios show negative gains — this is **expected and educational**. It demonstrates that "optimizations" are context-dependent:

| # | Scenario | Why optimized appears slower |
|:-:|----------|------------------------------|
| 1 | AllocationPressure (size=100) | Pre-allocated 1024-int buffer wastes memory relative to tiny 100-int workload; GC doesn't pressure at this scale |
| 6 | Substring vs AsSpan().Slice() | For a 5-char slice the CLR inlines and optimizes both paths identically; there is no allocation difference at this micro-scale |
| 8 | ArrayPool.Rent (1000) | Pool overhead (TLS slot lookup + locking) exceeds the cost of allocating a 1 KB array; pools pay off at high frequency or large sizes |
| 12 | IDisposable vs Finalizer | `Dispose()` runs synchronously within the benchmark loop; `CreateAndForgetNaive()` just creates the object — the finalizer runs later on the finalizer thread, making Naive *appear* faster in this isolated measurement |
| 13 | FrozenDictionary lookup | For a 100-key dictionary with constant-string keys, the JIT optimizes the standard `Dictionary<K,V>` perfectly. FrozenDictionary benefits are larger at scale or with dynamic input |
| 16 | SkipLocalsInit+Fill (256) | The JIT's optimizer already eliminates redundant zeroing in release builds; `[SkipLocalsInit]` is a no-op for this small buffer |
| 22 | Vector128 dot product | For 100-element arrays the overhead of SIMD setup, remainder handling, and horizontal reduction exceeds the gain. SIMD benefits are most visible at 1000+ elements |
| 26 | SimpleObjectPool | `ConcurrentBag<T>` has thread-safety overhead that exceeds the cost of `new StringBuilder()` for small counts. Production pool implementations (e.g., `Microsoft.Extensions.ObjectPool`) are more efficient |
| 28 | ValueTask sync path | `ValueTask<T>` returning from a fresh allocation is slower in the single-iteration Stopwatch test due to async state machine overhead that the warmup didn't fully absorb. `ValueTask` shines when the synchronous path is truly hot and avoids heap allocation. |

---

> Run `dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"` for rigorous BenchmarkDotNet statistics with proper warmup, multiple iterations, and confidence intervals.
