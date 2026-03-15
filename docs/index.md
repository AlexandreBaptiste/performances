# DotNet.Performance

Welcome to the **DotNet.Performance** documentation.

This project provides hands-on, runnable examples of .NET performance optimization techniques, organized by topic area. Each example demonstrates a `Naive` (unoptimized) implementation alongside an `Optimized` alternative with accompanying correctness tests and BenchmarkDotNet microbenchmarks.

## Topics

| Module | Topic |
|--------|-------|
| [01_MemoryModel](api/DotNet.Performance.Examples.MemoryModel.yml) | Stack vs heap, allocation pressure |
| [02_BoxingUnboxing](api/DotNet.Performance.Examples.BoxingUnboxing.yml) | Boxing costs, generic collections |
| [03_SpanAndMemory](api/DotNet.Performance.Examples.SpanAndMemory.yml) | Zero-copy slicing with Span\<T\> |
| [04_ArrayPool](api/DotNet.Performance.Examples.ArrayPool.yml) | Buffer renting and pooling |
| [05_RefStructs](api/DotNet.Performance.Examples.RefStructs.yml) | Stack-only ref structs, ref fields |
| [06_GarbageCollector](api/DotNet.Performance.Examples.GarbageCollector.yml) | GC generations, LOH, IDisposable |
| [07_GCRegionsAndPGO](api/DotNet.Performance.Examples.GCRegionsAndPGO.yml) | No-GC regions, frozen collections |
| [08_Inlining](api/DotNet.Performance.Examples.Inlining.yml) | MethodImpl inlining hints |
| [09_SkipLocalsInitAndUnsafe](api/DotNet.Performance.Examples.SkipLocalsInitAndUnsafe.yml) | SkipLocalsInit, Unsafe, NativeMemory |
| [10_Reflection](api/DotNet.Performance.Examples.Reflection.yml) | Reflection vs cached delegates |
| [11_SIMD](api/DotNet.Performance.Examples.SIMD.yml) | Vector\<T\> and Vector128 intrinsics |
| [12_StringOptimization](api/DotNet.Performance.Examples.StringOptimization.yml) | StringBuilder, SearchValues |
| [13_ObjectPooling](api/DotNet.Performance.Examples.ObjectPooling.yml) | Object pool patterns |
| [14_Patterns](api/DotNet.Performance.Examples.Patterns.yml) | CollectionsMarshal, ValueTask, struct layout |

## Getting Started

See the [README](../README.md) for setup instructions and how to run benchmarks.
