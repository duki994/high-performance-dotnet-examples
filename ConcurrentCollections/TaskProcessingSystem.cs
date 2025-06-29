using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConcurrentCollections;

public class TaskProcessingSystem
{
    /// <summary>
    ///     Store logs of all processed tasks
    /// </summary>
    private readonly ConcurrentBag<string> _processedTaskLogs = [];

    /// <summary>
    ///     Task lookup cache
    /// </summary>
    private readonly ConcurrentDictionary<int, string> _taskCache = new();

    /// <summary>
    ///     Simulate receiving and processing tasks
    /// </summary>
    /// <param name="taskCount">Number of received tasks</param>
    public async Task ProcessTasksAsynchronously(int taskCount)
    {
        Parallel.For(0, taskCount, async i =>
        {
            // Check if task is processed
            if (!_taskCache.ContainsKey(i))
            {
                // Simulate Task process
                var result = await ProcessTaskAsync(i);

                _taskCache.TryAdd(i, result);

                _processedTaskLogs.Add($"Task {i}: {result}");
            }
        });
    }

    private static Task<string> ProcessTaskAsync(int taskId)
    {
        // Simulate processing time
        Task.Delay(500).Wait();

        return Task.FromResult($"Processed Task: {taskId}");
    }

    public void PrintLogs()
    {
        foreach (var log in _processedTaskLogs) Console.WriteLine(log);
    }


    public string LookupTaskResult(int taskId)
    {
        if (_taskCache.TryGetValue(taskId, out var result)) return result;

        return $"Task {taskId} not found.";
    }
}