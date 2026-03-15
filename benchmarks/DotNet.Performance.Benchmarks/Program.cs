// ============================================================
// DotNet.Performance.Benchmarks
// Run (full BenchmarkDotNet): dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --filter "*"
// Run (quick Stopwatch comparison): dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks -- --quick-compare
// ============================================================

using BenchmarkDotNet.Running;
using DotNet.Performance.Benchmarks;

// Quick comparison mode — fast Stopwatch-based measurement of all Naive/Optimized pairs.
if (args.Length == 1 && args[0] == "--quick-compare")
{
    QuickComparisonRunner.Run();
    return;
}

// Guard: BenchmarkDotNet requires Release mode and no debugger attached for valid results.
// Running in Debug mode will produce incorrect (inflated) numbers.
if (System.Diagnostics.Debugger.IsAttached)
{
    Console.Error.WriteLine("WARNING: Debugger is attached. Benchmark results will not be accurate.");
    Console.Error.WriteLine("Run with: dotnet run -c Release --project benchmarks/DotNet.Performance.Benchmarks");
}

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);
