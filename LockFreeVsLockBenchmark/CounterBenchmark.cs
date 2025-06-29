using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace LockFreeVsLockBenchmark;

public class CounterBenchmark
{
    private LockBasedCounter _lockBasedCounter;
    private LockFreeCounter _lockFreeCounter;
    // Define the number of increments and threads for the benchmark
    private const int NumIncrements = 1_000_000;
    private const int NumThreads = 8;
    
    
    [GlobalSetup]
    public void Setup()
    {
        _lockBasedCounter = new LockBasedCounter();
        _lockFreeCounter = new LockFreeCounter();
    }

    [Benchmark]
    public void LockFreeTest()
    {
        Parallel.For(0, NumThreads, _ =>
        {
            for (var i = 0; i < NumIncrements / NumThreads; i++)
            {
                _lockFreeCounter.Increment();
            }
        });
    }
    
    [Benchmark]
    public void LockBasedTest()
    {
        Parallel.For(0, NumThreads, _ =>
        {
            for (var i = 0; i < NumIncrements / NumThreads; i++)
            {
                _lockBasedCounter.Increment();
            }
        });
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}