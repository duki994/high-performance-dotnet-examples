using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelCollections;

public class ListProcessing
{
    private readonly List<int> _data;
    private readonly int[] _results;

    public ListProcessing(List<int> data)
    {
        _data = data;
        _results = new int[data.Count];
    }

    public void ProcessData()
    {
        Parallel.For(0, _data.Count, i =>
        {
            // Simulate some processing
            _results[i] = Compute(_data[i]);
        });
    }

    private int Compute(int value)
    {
        // Simulate some computation
        return value * value;
    }
}