using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

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