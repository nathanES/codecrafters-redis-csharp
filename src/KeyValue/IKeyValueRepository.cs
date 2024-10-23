namespace codecrafters_redis.KeyValue;

public interface IKeyValueRepository : IDisposable
{
   Task SetAsync(string key, string value, int? expirationMilliseconds = null);
   Task<string?> GetAsync(string key);
   Task DeleteAsync(string key);
}