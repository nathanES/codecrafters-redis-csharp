using System.Collections.Concurrent;

namespace codecrafters_redis.Configuration;

public class InMemoryConfigRepository : IConfigRepository
{
    private readonly ConcurrentDictionary<string, string> _configDictionary = new ConcurrentDictionary<string, string>();

    public InMemoryConfigRepository()
    {
        
    }

    public async Task SetAsync(string key, string value)
    {
        var result = _configDictionary.TryAdd(key, value);
        Console.WriteLine($"ConfigAdded : {result.ToString()}");
    }

    public async Task<string?> GetAsync(string key)
    {
        var result = _configDictionary.TryGetValue(key, out string? value);
        Console.WriteLine($"ConfigRetrieved : {result.ToString()}");
        return value;
    }

    public async Task DeleteAsync(string key)
    {
        var result = _configDictionary.TryRemove(key, out _);
        Console.WriteLine($"ConfigRemoved : {result.ToString()}");
    }
}