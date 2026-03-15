## Performance Comparison Results

> Generated 2026-03-15 21:12 · .NET 10.0.4 · 5 000 iterations per method (Stopwatch, Release build)

| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |
|:-:|:---------|:---------|:---:|:---:|:---:|
| 1 | 01 MemoryModel | AllocationPressure (size=100) | 1813,52 | 2499,48 | -37,8% ⚠️ |
| 2 | 01 MemoryModel | AllocationPressure (size=1000) | 13006,08 | 6926,62 | **+46,7%** ✅ |
| 3 | 02 BoxingUnboxing | ArrayList vs List<int> (100) | 7970,66 | 2910,54 | **+63,5%** ✅ |
| 4 | 02 BoxingUnboxing | ArrayList vs List<int> (1000) | 33386,98 | 1964,34 | **+94,1%** ✅ |
| 5 | 02 BoxingUnboxing | IValue interface vs generic (1000) | 33698,90 | 2768,42 | **+91,8%** ✅ |
| 6 | 03 SpanAndMemory | Substring vs AsSpan().Slice() | 26,18 | 39,40 | -50,5% ⚠️ |
| 7 | 03 SpanAndMemory | new byte[] vs stackalloc (256) | 3806,58 | 220,76 | **+94,2%** ✅ |
| 8 | 04 ArrayPool | new byte[] vs ArrayPool.Rent (1000) | 8200,72 | 7664,62 | **+6,5%** ✅ |
| 9 | 04 ArrayPool | new byte[] vs MemoryPool.Rent (1000) | 15054,72 | 15471,28 | -2,8% ⚠️ |
| 10 | 05 RefStructs | new int[] vs stackalloc+ref struct (64) | 917,70 | 476,34 | **+48,1%** ✅ |
| 11 | 06 GarbageCollector | new byte[1000] vs ArrayPool (GC pressure) | 8035,28 | 7172,14 | **+10,7%** ✅ |
| 12 | 06 GarbageCollector | IDisposable vs Finalizer | 55,56 | 153,12 | -175,6% ⚠️ |
| 13 | 07 GCRegionsAndPGO | Dictionary vs FrozenDictionary lookup | 34,52 | 78,24 | -126,7% ⚠️ |
| 14 | 08 Inlining | Default vs AggressiveInlining (x=42) | 9,00 | 9,52 | -5,8% ⚠️ |
| 15 | 08 Inlining | Default vs AggressiveOptimization (10k elems) | 26700,54 | 3178,78 | **+88,1%** ✅ |
| 16 | 09 SkipLocalsInit | Clear+Fill vs SkipLocalsInit+Fill (256) | 218,54 | 152,40 | **+30,3%** ✅ |
| 17 | 09 SkipLocalsInit | Manual offset copy vs Unsafe.Add copy | 785,92 | 658,30 | **+16,2%** ✅ |
| 18 | 10 Reflection | PropertyInfo.GetValue vs cached Func<> | 154,52 | 17,46 | **+88,7%** ✅ |
| 19 | 10 Reflection | PropertyInfo.GetValue vs ExpressionTree | 109,48 | 23,22 | **+78,8%** ✅ |
| 20 | 10 Reflection | Activator.CreateInstance vs new T() | 50,84 | 38,44 | **+24,4%** ✅ |
| 21 | 11 SIMD | ScalarSum vs Vector<int> (10000 ints) | 26489,36 | 8479,82 | **+68,0%** ✅ |
| 22 | 11 SIMD | DotProduct scalar vs Vector128 (100 floats) | 685,32 | 1489,18 | -117,3% ⚠️ |
| 23 | 12 StringOpt | string+= vs StringBuilder (50 segments) | 1933,52 | 972,44 | **+49,7%** ✅ |
| 24 | 12 StringOpt | string.Format vs interpolation | 109,40 | 97,98 | **+10,4%** ✅ |
| 25 | 12 StringOpt | IndexOfAny vs SearchValues<char> | 216,52 | 94,34 | **+56,4%** ✅ |
| 26 | 13 ObjectPool | new StringBuilder vs SimpleObjectPool (100 ops) | 4937,56 | 14635,40 | -196,4% ⚠️ |
| 27 | 14 Patterns | LINQ Sum vs CollectionsMarshal.AsSpan (10k) | 32280,76 | 3794,24 | **+88,2%** ✅ |
| 28 | 14 Patterns | Task<T> vs ValueTask<T> (sync hot path) | 42,80 | 897,08 | -1996,0% ⚠️ |

> **Gain** = (Naive − Optimized) / Naive × 100. Positive = optimized is faster.
> These are approximate single-process Stopwatch measurements. Run BenchmarkDotNet for rigorous statistics:
> ```bash
> dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"
> ```
