namespace LockFreeVsLockBenchmark;

public class LockBasedCounter
{
    private readonly object _lock = new();
    private int _count;

    public void Increment()
    {
        lock (_lock)
        {
            _count++;
        }
    }

    public int GetCount()
    {
        lock (_lock)
        {
            return _count;
        }
    }
}