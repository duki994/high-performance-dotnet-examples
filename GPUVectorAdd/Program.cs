
using System;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.OpenCL;

namespace GPUVectorAdd;

public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance;
        var summary = BenchmarkRunner.Run<VectorAdditionBenchmark>(config, args);

        // Use this to select benchmarks from the console:
        // var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}