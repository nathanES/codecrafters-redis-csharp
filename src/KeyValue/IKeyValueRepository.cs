using codecrafters_redis.Common;

namespace codecrafters_redis.KeyValue;

public interface IKeyValueRepository : IDisposable,IRepository
{
   Task SetAsync(string key, string value, int? expirationMilliseconds = null);
   Task<string?> GetAsync(string key);
   Task DeleteAsync(string key);
}