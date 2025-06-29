namespace LockFreeVsLockBenchmark;

public class LockBasedCounter
{
    private int _count = 0;
    private readonly object _lock = new object();
    
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
