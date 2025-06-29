using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ParallelCollections;

public class PartitionedProcessing
{
    private readonly int[] _data;
    private readonly int[] _results;

    public PartitionedProcessing(int[] data)
    {
        _data = data;
        _results = new int[data.Length];
    }

    public void ProcessData()
    {
        var rangePartitioner = Partitioner.Create(0, _data.Length, 1000);

        Parallel.ForEach(rangePartitioner, range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
            {
                _results[i] = Compute(_data[i]);
            }
        });
    }

    private int Compute(int value)
    {
        // Simulate some computation
        return value * value;
    }
}