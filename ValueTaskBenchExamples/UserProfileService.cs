using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ValueTaskBenchExamples;

public class UserProfile
{
    public int UserId { get; set; }
    public string Name { get; set; }
}

public class UserProfileService
{
    private readonly Dictionary<int, UserProfile> _cache = new();
    private readonly Random _random = new();

    public async Task<UserProfile> GetUserProfileTaskAsync(int userId)
    {
        if (_cache.TryGetValue(userId, out var profile)) return profile;

        // Cache miss for 10% of times - simulated
        if (_random.Next(10) == 0)
        {
            profile = await FetchFromDatabaseAsync(userId);
            _cache[userId] = profile;
        }
        else
        {
            profile = new UserProfile { UserId = userId, Name = "Cached User" };
        }

        return profile;
    }

    public async ValueTask<UserProfile> GetUserProfileValueTaskAsync(int userId)
    {
        if (_cache.TryGetValue(userId, out var profile))
            // return synchronously - ValueTask allocations should help
            return profile;

        // Cache miss for 10% of times - simulated
        if (_random.Next(10) == 0)
        {
            profile = await FetchFromDatabaseAsync(userId);
            _cache[userId] = profile;
        }
        else
        {
            profile = new UserProfile { UserId = userId, Name = "Cached User" };
        }

        return profile;
    }


    private Task<UserProfile> FetchFromDatabaseAsync(int userId)
    {
        // Simulate DB call
        return Task.FromResult(new UserProfile { UserId = userId, Name = "Database User" });
    }
}