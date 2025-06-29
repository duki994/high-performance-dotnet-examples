using System.Threading;

namespace LockFreeVsLockBenchmark;

public class LockFreeCounter
{
    private int _count = 0;

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }
    
    public int GetCount()
    {
        return _count;
    }
}