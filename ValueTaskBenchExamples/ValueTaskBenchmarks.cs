using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace ValueTaskBenchExamples;

[MemoryDiagnoser]
public class ValueTaskBenchmarks
{
    private readonly UserProfileService _userProfileService = new();


    [Benchmark]
    public async Task<UserProfile> UsingTask()
    {
        return await _userProfileService.GetUserProfileTaskAsync(1);
    }

    [Benchmark]
    public async ValueTask<UserProfile> UsingValueTask()
    {
        return await _userProfileService.GetUserProfileValueTaskAsync(1);
    }
}