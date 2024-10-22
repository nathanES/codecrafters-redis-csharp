namespace codecrafters_redis.KeyValue;

public interface IKeyValueRepository
{
   Task SetAsync(string key, string value);
   Task<string?> GetAsync(string key);
}