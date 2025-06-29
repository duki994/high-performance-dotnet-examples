using System;
using System.Threading.Tasks;

namespace ConcurrentCollections;

public class Program
{
    public static async Task Main(string[] args)
    {
        // var config = DefaultConfig.Instance;
        // var summary = BenchmarkRunner.Run<Benchmarks>(config, args);

        // Use this to select benchmarks from the console:
        // var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

        var taskProcessingSystem = new TaskProcessingSystem();
        // Process 10 tasks concurrently
        await taskProcessingSystem.ProcessTasksAsynchronously(10);
        // // Print the logs of all processed tasks
        taskProcessingSystem.PrintLogs();
        // Check the result of a specific task
        Console.WriteLine(taskProcessingSystem.LookupTaskResult(3));
    }
}