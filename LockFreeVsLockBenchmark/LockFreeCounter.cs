using System.Threading;

namespace LockFreeVsLockBenchmark;

public class LockFreeCounter
{
    private int _count;

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public int GetCount()
    {
        return _count;
    }
}