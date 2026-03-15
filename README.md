# DotNet.Performance

A comprehensive .NET 10 performance examples library, demonstrating low-level performance optimization techniques through paired Naive/Optimized implementations, correctness tests, and BenchmarkDotNet microbenchmarks.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Solution Structure](#solution-structure)
- [Running Tests](#running-tests)
- [Running Benchmarks](#running-benchmarks)
- [Performance Results](#performance-results)
- [Topics Covered](#topics-covered)
- [Architecture](#architecture)
- [Notes & Caveats](#notes--caveats)

---

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 10.0.x (`global.json` pins the exact version) |
| C# | 14 (preview features enabled) |

```bash
dotnet --version   # should print 10.0.x
```

---

## Solution Structure

```
DotNet.Performance.slnx
├── src/
│   └── DotNet.Performance.Examples/          # Class library — all demo classes
│       ├── 01_MemoryModel/
│       ├── 02_BoxingUnboxing/
│       ├── 03_SpanAndMemory/
│       ├── 04_ArrayPool/
│       ├── 05_RefStructs/
│       ├── 06_GarbageCollector/
│       ├── 07_GCRegionsAndPGO/
│       ├── 08_Inlining/
│       ├── 09_SkipLocalsInitAndUnsafe/
│       ├── 10_Reflection/
│       ├── 11_SIMD/
│       ├── 12_StringOptimization/
│       ├── 13_ObjectPooling/
│       └── 14_Patterns/
├── tests/
│   └── DotNet.Performance.Tests/             # xUnit correctness tests
└── benchmarks/
    └── DotNet.Performance.Benchmarks/        # BenchmarkDotNet microbenchmarks
```

---

## Running Tests

```bash
# Run all correctness tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a specific category
dotnet test --filter "FullyQualifiedName~MemoryModel"
```

---

## Running Benchmarks

> **Important**: Benchmarks **must** be run in Release mode. Debug builds produce meaningless results because the JIT applies far fewer optimizations.

```bash
# Run all benchmarks
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"

# Run a specific category
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Allocation*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Boxing*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Span*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*ArrayPool*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*GC*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Inlining*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Reflection*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*SIMD*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*String*"
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Pool*"

# With disassembly output (requires OS-specific disassembler: objdump on Linux/mac, JitDasm on Windows)
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*Inlining*" --disassembly

# List all available benchmarks without running
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --list flat
```

---

## Performance Results

A quick Stopwatch-based comparison of every Naive/Optimized pair is available in [quick-comparison.md](quick-comparison.md).

Re-run it at any time (Release build required):

```bash
dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --quick-compare
```

Highlights from the last run (.NET 10.0.4, 5,000 iterations per method):

| # | Category | Scenario | Naive (ns/op) | Optimized (ns/op) | Gain |
|:-:|:---------|:---------|:---:|:---:|:---:|
| 4 | 02 BoxingUnboxing | ArrayList vs List\<int\> (1000) | 35,217 | 2,550 | **+92.8%** ✅ |
| 5 | 02 BoxingUnboxing | IValue interface vs generic (1000) | 40,049 | 941 | **+97.6%** ✅ |
| 7 | 03 SpanAndMemory | new byte[] vs stackalloc (256) | 4,696 | 516 | **+89.0%** ✅ |
| 15 | 08 Inlining | Default vs AggressiveOptimization (10k) | 26,833 | 4,473 | **+83.3%** ✅ |
| 18 | 10 Reflection | PropertyInfo.GetValue vs cached Func\<\> | 105 | 16 | **+84.7%** ✅ |
| 19 | 10 Reflection | PropertyInfo.GetValue vs ExpressionTree | 130 | 15 | **+88.1%** ✅ |
| 27 | 14 Patterns | LINQ Sum vs CollectionsMarshal.AsSpan (10k) | 45,125 | 5,230 | **+88.4%** ✅ |

> Some scenarios show negative gains (e.g. ArrayPool at small sizes, FrozenDictionary on 100 keys). This is intentional — the ⚠️ cases demonstrate that optimizations are **workload-scale-dependent**. See [quick-comparison.md](quick-comparison.md) for the full table and explanations.

---

## Topics Covered

| # | Topic | Key Concept | Naive | Optimized |
|---|-------|-------------|-------|-----------|
| 01 | Memory Model & Allocation | Stack vs heap | New array per call | Reuse buffer |
| 02 | Boxing & Unboxing | `object` vs `List<T>` | `ArrayList` + boxing | `List<int>` |
| 03 | Span\<T\> & Memory\<T\> | Zero-copy slicing | `Substring` | `AsSpan().Slice()` |
| 04 | ArrayPool\<T\> | Buffer reuse | `new byte[]` | `ArrayPool.Shared.Rent/Return` |
| 05 | ref struct | Stack-only types | `new T[]` | `ref struct` + `stackalloc` |
| 06 | Garbage Collector | GC generations, LOH, finalizers | Finalizer every object | `IDisposable` + `GC.SuppressFinalize` |
| 07 | GC Regions & PGO | No-GC regions, frozen collections | `Dictionary<K,V>` | `GC.TryStartNoGCRegion` / `FrozenDictionary` |
| 08 | Inlining | `[MethodImpl]` hints | Default inlining | `AggressiveInlining` |
| 09 | SkipLocalsInit & Unsafe | Stack zero suppression | Zeroed stackalloc | `[SkipLocalsInit]` + write-before-read |
| 10 | Reflection | Runtime metadata | `MethodInfo.Invoke` | Cached `Func<>` / Expression trees |
| 11 | SIMD & Vectorization | `Vector<T>` / `Vector128<T>` | Scalar loop | `Vector<int>` SIMD sum |
| 12 | String Optimization | `StringBuilder`, `SearchValues` | `+=` in loop | `StringBuilder` / `SearchValues<char>` |
| 13 | Object Pooling | Pool and reuse objects | `new T()` per request | `SimpleObjectPool<T>` |
| 14 | Advanced Patterns | `CollectionsMarshal`, `ValueTask`, struct layout | LINQ / `Task<T>` | `CollectionsMarshal.AsSpan` / `ValueTask<T>` |

---

## Architecture

Every source file follows the same structure:

```csharp
// ============================================================
// Concept  : <topic name>
// Summary  : <one-line description>
// When to use   : <guidance>
// When NOT to use: <anti-pattern warning>
// ============================================================

public sealed class SomePerfDemo
{
    /// <summary>The unoptimized implementation for comparison.</summary>
    public ResultType Naive(...) { ... }

    /// <summary>The optimized implementation.</summary>
    public ResultType Optimized(...) { ... }
}
```

Corresponding benchmark:

```csharp
[MemoryDiagnoser]
[RankColumn]
public class SomePerfBenchmark
{
    [Benchmark(Baseline = true)]
    public ResultType Naive() => _demo.Naive(...);

    [Benchmark]
    public ResultType Optimized() => _demo.Optimized(...);
}
```

---

## Notes & Caveats

### BenchmarkDotNet requires Release mode (RISK-001)

Always use `-c Release`. Debug builds bypass JIT optimizations and produce misleading numbers. A runtime guard in `Program.cs` will warn when a debugger is attached.

### Unsafe code is educational only (RISK-002)

Files in `09_SkipLocalsInitAndUnsafe/` are marked `// UNSAFE — educational only`. The `AllowUnsafeBlocks` property is enabled in the Examples project. Do not copy unsafe patterns into production code without a full security review.

### Dynamic PGO results vary (RISK-003)

Benchmarks in `07_GCRegionsAndPGO/` annotated with `[BenchmarkCategory("PGO")]` are sensitive to warm-up iterations and machine characteristics. Enable Dynamic PGO with:

```bash
DOTNET_TieredPGO=1 dotnet run -c Release ...
```

### SIMD availability varies by CPU (RISK-004)

SIMD benchmarks in `11_SIMD/` check `Vector.IsHardwareAccelerated` and `Vector128.IsHardwareAccelerated` at runtime. On machines without AVX2/SSE2 support the code automatically falls back to scalar paths. AVX2 (Intel Haswell 2013+ / AMD Zen 2019+) is recommended for best results.

### GC mode affects GC benchmark results (RISK-005)

GC generation behavior differs between Workstation GC (default in console apps) and Server GC. Check `GCSettings.IsServerGC` at runtime. The `GCSettingsDemo.IsServerGC()` helper exposes this. Set Server GC in `runtimeconfig.json`:

```json
{
  "configProperties": {
    "System.GC.Server": true
  }
}
```

---

## DocFX Documentation

```bash
# Install DocFX (requires .NET tool)
dotnet tool install -g docfx

# Build API docs
docfx docs/docfx.json --serve
```

Browse to `http://localhost:8080` to view the generated API documentation.
