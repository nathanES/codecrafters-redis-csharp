using codecrafters_redis.Common;

namespace codecrafters_redis.Configuration;

public interface IConfigRepository : IRepository
{
    Task SetAsync(string key, string value);
    Task<string?> GetAsync(string key);
    Task DeleteAsync(string key); 
}