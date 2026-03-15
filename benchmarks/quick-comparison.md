## Performance Comparison Results

> Generated 2026-03-15 20:52 · .NET 10.0.4 · 5 000 iterations per method (Stopwatch, Release build)

| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |
|:-:|:---------|:---------|:---:|:---:|:---:|
| 1 | 01 MemoryModel | AllocationPressure (size=100) | 1672,50 | 2466,04 | -47,4% ⚠️ |
| 2 | 01 MemoryModel | AllocationPressure (size=1000) | 10991,84 | 8971,20 | **+18,4%** ✅ |
| 3 | 02 BoxingUnboxing | ArrayList vs List<int> (100) | 7825,20 | 2829,68 | **+63,8%** ✅ |
| 4 | 02 BoxingUnboxing | ArrayList vs List<int> (1000) | 35217,04 | 2550,36 | **+92,8%** ✅ |
| 5 | 02 BoxingUnboxing | IValue interface vs generic (1000) | 40049,58 | 941,68 | **+97,6%** ✅ |
| 6 | 03 SpanAndMemory | Substring vs AsSpan().Slice() | 53,48 | 60,18 | -12,5% ⚠️ |
| 7 | 03 SpanAndMemory | new byte[] vs stackalloc (256) | 4696,32 | 516,60 | **+89,0%** ✅ |
| 8 | 04 ArrayPool | new byte[] vs ArrayPool.Rent (1000) | 10507,40 | 12541,38 | -19,4% ⚠️ |
| 9 | 04 ArrayPool | new byte[] vs MemoryPool.Rent (1000) | 15627,88 | 14656,66 | **+6,2%** ✅ |
| 10 | 05 RefStructs | new int[] vs stackalloc+ref struct (64) | 716,50 | 422,02 | **+41,1%** ✅ |
| 11 | 06 GarbageCollector | new byte[1000] vs ArrayPool (GC pressure) | 8128,24 | 5969,02 | **+26,6%** ✅ |
| 12 | 06 GarbageCollector | IDisposable vs Finalizer | 59,44 | 134,16 | -125,7% ⚠️ |
| 13 | 07 GCRegionsAndPGO | Dictionary vs FrozenDictionary lookup | 35,26 | 84,70 | -140,2% ⚠️ |
| 14 | 08 Inlining | Default vs AggressiveInlining (x=42) | 11,40 | 11,14 | **+2,3%** ✅ |
| 15 | 08 Inlining | Default vs AggressiveOptimization (10k elems) | 26833,16 | 4473,84 | **+83,3%** ✅ |
| 16 | 09 SkipLocalsInit | Clear+Fill vs SkipLocalsInit+Fill (256) | 203,28 | 221,78 | -9,1% ⚠️ |
| 17 | 09 SkipLocalsInit | Manual offset copy vs Unsafe.Add copy | 672,76 | 503,38 | **+25,2%** ✅ |
| 18 | 10 Reflection | PropertyInfo.GetValue vs cached Func<> | 105,14 | 16,10 | **+84,7%** ✅ |
| 19 | 10 Reflection | PropertyInfo.GetValue vs ExpressionTree | 130,18 | 15,46 | **+88,1%** ✅ |
| 20 | 10 Reflection | Activator.CreateInstance vs new T() | 52,22 | 44,28 | **+15,2%** ✅ |
| 21 | 11 SIMD | ScalarSum vs Vector<int> (10000 ints) | 33445,24 | 16065,50 | **+52,0%** ✅ |
| 22 | 11 SIMD | DotProduct scalar vs Vector128 (100 floats) | 796,90 | 2030,00 | -154,7% ⚠️ |
| 23 | 12 StringOpt | string+= vs StringBuilder (50 segments) | 3285,32 | 1313,08 | **+60,0%** ✅ |
| 24 | 12 StringOpt | string.Format vs interpolation | 117,90 | 90,54 | **+23,2%** ✅ |
| 25 | 12 StringOpt | IndexOfAny vs SearchValues<char> | 275,20 | 114,40 | **+58,4%** ✅ |
| 26 | 13 ObjectPool | new StringBuilder vs SimpleObjectPool (100 ops) | 5769,28 | 18747,02 | -224,9% ⚠️ |
| 27 | 14 Patterns | LINQ Sum vs CollectionsMarshal.AsSpan (10k) | 45125,14 | 5230,38 | **+88,4%** ✅ |
| 28 | 14 Patterns | Task<T> vs ValueTask<T> (sync hot path) | 77,40 | 1659,30 | -2043,8% ⚠️ |

> **Gain** = (Naive − Optimized) / Naive × 100. Positive = optimized is faster.
> These are approximate single-process Stopwatch measurements. Run BenchmarkDotNet for rigorous statistics:
> ```bash
> dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"
> ```
