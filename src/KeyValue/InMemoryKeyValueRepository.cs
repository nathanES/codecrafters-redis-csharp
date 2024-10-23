namespace codecrafters_redis.KeyValue;

public class InMemoryKeyValueRepository : IKeyValueRepository
{
    private Dictionary<string, string> _keyValueDictionary = new Dictionary<string, string>();
    public Task SetAsync(string key, string value)
    {
        var isKeyAdded = _keyValueDictionary.TryAdd(key, value);
        Console.WriteLine($"keyAdded : {isKeyAdded.ToString()}");
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string key)
    {
        var isKeyGot = _keyValueDictionary.TryGetValue(key, out var value);

        Console.WriteLine($"keyGot : {isKeyGot.ToString()}");

        return Task.FromResult(value); 
    }
}