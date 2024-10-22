namespace codecrafters_redis.KeyValue;

public class InMemoryKeyValueRepository : IKeyValueRepository
{
    private Dictionary<string, string> _keyValueDictionary = new Dictionary<string, string>();
    public Task SetAsync(string key, string value)
    {
        _keyValueDictionary.TryAdd(key, value);
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string key)
    {
        _keyValueDictionary.TryGetValue(key, out var value);
        return Task.FromResult(value); 
    }
}