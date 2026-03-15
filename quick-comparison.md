## Performance Comparison Results

> Generated 2026-03-15 · .NET 10.0.4 on Windows 10.0.26200 · 5,000 iterations per method (Stopwatch, Release build)

| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |
|:-:|:---------|:---------|:---:|:---:|:---:|
| 1 | 01 MemoryModel | AllocationPressure (size=100) | 1,813.52 | 2,499.48 | -37.8% ⚠️ |
| 2 | 01 MemoryModel | AllocationPressure (size=1000) | 13,006.08 | 6,926.62 | **+46.7%** ✅ |
| 3 | 02 BoxingUnboxing | ArrayList vs List&lt;int&gt; (100) | 7,970.66 | 2,910.54 | **+63.5%** ✅ |
| 4 | 02 BoxingUnboxing | ArrayList vs List&lt;int&gt; (1000) | 33,386.98 | 1,964.34 | **+94.1%** ✅ |
| 5 | 02 BoxingUnboxing | IValue interface vs generic (1000) | 33,698.90 | 2,768.42 | **+91.8%** ✅ |
| 6 | 03 SpanAndMemory | Substring vs AsSpan().Slice() | 26.18 | 39.40 | -50.5% ⚠️ |
| 7 | 03 SpanAndMemory | new byte[] vs stackalloc (256) | 3,806.58 | 220.76 | **+94.2%** ✅ |
| 8 | 04 ArrayPool | new byte[] vs ArrayPool.Rent (1000) | 8,200.72 | 7,664.62 | **+6.5%** ✅ |
| 9 | 04 ArrayPool | new byte[] vs MemoryPool.Rent (1000) | 15,054.72 | 15,471.28 | -2.8% ⚠️ |
| 10 | 05 RefStructs | new int[] vs stackalloc+ref struct (64) | 917.70 | 476.34 | **+48.1%** ✅ |
| 11 | 06 GarbageCollector | new byte[1000] vs ArrayPool (GC pressure) | 8,035.28 | 7,172.14 | **+10.7%** ✅ |
| 12 | 06 GarbageCollector | IDisposable vs Finalizer | 55.56 | 153.12 | -175.6% ⚠️ |
| 13 | 07 GCRegionsAndPGO | Dictionary vs FrozenDictionary lookup | 34.52 | 78.24 | -126.7% ⚠️ |
| 14 | 08 Inlining | Default vs AggressiveInlining (x=42) | 9.00 | 9.52 | -5.8% ⚠️ |
| 15 | 08 Inlining | Default vs AggressiveOptimization (10k elems) | 26,700.54 | 3,178.78 | **+88.1%** ✅ |
| 16 | 09 SkipLocalsInit | Clear+Fill vs SkipLocalsInit+Fill (256) | 218.54 | 152.40 | **+30.3%** ✅ |
| 17 | 09 SkipLocalsInit | Manual offset copy vs Unsafe.Add copy | 785.92 | 658.30 | **+16.2%** ✅ |
| 18 | 10 Reflection | PropertyInfo.GetValue vs cached Func&lt;&gt; | 154.52 | 17.46 | **+88.7%** ✅ |
| 19 | 10 Reflection | PropertyInfo.GetValue vs ExpressionTree | 109.48 | 23.22 | **+78.8%** ✅ |
| 20 | 10 Reflection | Activator.CreateInstance vs new T() | 50.84 | 38.44 | **+24.4%** ✅ |
| 21 | 11 SIMD | ScalarSum vs Vector&lt;int&gt; (10000 ints) | 26,489.36 | 8,479.82 | **+68.0%** ✅ |
| 22 | 11 SIMD | DotProduct scalar vs Vector128 (100 floats) | 685.32 | 1,489.18 | -117.3% ⚠️ |
| 23 | 12 StringOpt | string+= vs StringBuilder (50 segments) | 1,933.52 | 972.44 | **+49.7%** ✅ |
| 24 | 12 StringOpt | string.Format vs interpolation | 109.40 | 97.98 | **+10.4%** ✅ |
| 25 | 12 StringOpt | IndexOfAny vs SearchValues&lt;char&gt; | 216.52 | 94.34 | **+56.4%** ✅ |
| 26 | 13 ObjectPool | new StringBuilder vs SimpleObjectPool (100 ops) | 4,937.56 | 14,635.40 | -196.4% ⚠️ |
| 27 | 14 Patterns | LINQ Sum vs CollectionsMarshal.AsSpan (10k) | 32,280.76 | 3,794.24 | **+88.2%** ✅ |
| 28 | 14 Patterns | Task&lt;T&gt; vs ValueTask&lt;T&gt; (sync hot path) | 42.80 | 897.08 | -1996.0% ⚠️ |

> **Gain** = (Naive − Optimized) / Naive × 100. Positive = optimized is faster.

---

## Notes on ⚠️ Results

Several scenarios show negative gains — this is **expected and educational**. It demonstrates that "optimizations" are context-dependent:

| # | Scenario | Why optimized appears slower |
|:-:|----------|------------------------------|
| 1 | AllocationPressure (size=100) | Pre-allocated 1024-int buffer wastes memory relative to tiny 100-int workload; GC doesn't pressure at this scale |
| 6 | Substring vs AsSpan().Slice() | For a 5-char slice the CLR inlines and optimizes both paths identically; there is no allocation difference at this micro-scale |
| 9 | MemoryPool.Rent (1000) | `MemoryPool<T>` has higher overhead than `ArrayPool<T>` due to `IMemoryOwner<T>` wrapping; it pays off with larger buffers or when `Memory<T>` semantics are needed |
| 12 | IDisposable vs Finalizer | `Dispose()` runs synchronously within the benchmark loop; `CreateAndForgetNaive()` just creates the object — the finalizer runs later on the finalizer thread, making Naive *appear* faster in this isolated measurement |
| 13 | FrozenDictionary lookup | For a 100-key dictionary with constant-string keys, the JIT optimizes the standard `Dictionary<K,V>` perfectly. FrozenDictionary benefits are larger at scale or with dynamic input |
| 14 | AggressiveInlining (x=42) | The JIT already inlines simple methods by default; `[MethodImpl(MethodImplOptions.AggressiveInlining)]` adds no benefit for trivially small methods |
| 22 | Vector128 dot product | For 100-element arrays the overhead of SIMD setup, remainder handling, and horizontal reduction exceeds the gain. SIMD benefits are most visible at 1000+ elements |
| 26 | SimpleObjectPool | `ConcurrentBag<T>` has thread-safety overhead that exceeds the cost of `new StringBuilder()` for small counts. Production pool implementations (e.g., `Microsoft.Extensions.ObjectPool`) are more efficient |
| 28 | ValueTask sync path | `ValueTask<T>` returning from a fresh allocation is slower in the single-iteration Stopwatch test due to async state machine overhead that the warmup didn't fully absorb. `ValueTask` shines when the synchronous path is truly hot and avoids heap allocation. |

---

> Run `dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"` for rigorous BenchmarkDotNet statistics with proper warmup, multiple iterations, and confidence intervals.
