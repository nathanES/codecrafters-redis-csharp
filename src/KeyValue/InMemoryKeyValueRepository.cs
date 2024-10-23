using System.Collections.Concurrent;

namespace codecrafters_redis.KeyValue;

public class InMemoryKeyValueRepository : IKeyValueRepository, IDisposable
{
    private ConcurrentDictionary<string, string> _keyValueDictionary = new ConcurrentDictionary<string, string>();
    private ConcurrentDictionary<string, DateTime?> _expiryDictionary = new ConcurrentDictionary<string, DateTime?>();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly Task _cleanUpDictionaries;
    public InMemoryKeyValueRepository()
    {
        // Start a background task to clean up expired keys periodically
        _cleanUpDictionaries = Task.Run(() => CleanupExpiredKeys(_cancellationTokenSource.Token));
    }
    public Task SetAsync(string key, string value, int? expirationMillisecond = null)
    {
        var isKeyAdded = _keyValueDictionary.TryAdd(key, value);
        Console.WriteLine($"isKeyAdded : {isKeyAdded.ToString()}");
        if (expirationMillisecond.HasValue)
        {
            var isExpiryAdded = _expiryDictionary.TryAdd(key, DateTime.UtcNow.AddMilliseconds(expirationMillisecond.Value));
            Console.WriteLine($"isExpiryAdded : {isExpiryAdded.ToString()} With expiration");
        }
        else
        {
            var isExpiryAdded = _expiryDictionary.TryAdd(key, null); 
            Console.WriteLine($"isExpiryAdded : {isExpiryAdded.ToString()} With no expiration");
        }
        return Task.CompletedTask;
    }
    public Task<string?> GetAsync(string key)
    {
        if (_keyValueDictionary.TryGetValue(key, out string? value))
        {
            if (_expiryDictionary.TryGetValue(key, out var expiry)
                && expiry.HasValue
                && expiry.Value <= DateTime.UtcNow)
            {
                Console.WriteLine($"Key '{key}' has expired.");
                _expiryDictionary.TryRemove(key, out _);
                _keyValueDictionary.TryRemove(key, out _);
                return Task.FromResult<string?>(null);
            }

            Console.WriteLine($"Key '{key}' retrieved.");
            return Task.FromResult<string?>(value);
        }

        Console.WriteLine($"Key '{key}' not found.");
        return Task.FromResult<string?>(value); 
    }
    public Task DeleteAsync(string key)
    {
        var isKeyDeleted = _keyValueDictionary.TryRemove(key, out _);
        _expiryDictionary.TryRemove(key, out _);
        Console.WriteLine($"Key '{key}' deleted: {isKeyDeleted}");
        return Task.CompletedTask;
    }
    private async Task CleanupExpiredKeys(CancellationToken cancellationToken)
    {
        Console.WriteLine("Background expiration cleanup task started.");
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var key in _expiryDictionary.Keys)
            {
                if (_expiryDictionary.TryGetValue(key, out var expiry) && expiry.HasValue)
                {
                    if (expiry.Value <= DateTime.UtcNow)
                    {
                        Console.WriteLine($"Removing expired key '{key}'");
                        _keyValueDictionary.TryRemove(key, out _);
                        _expiryDictionary.TryRemove(key, out _);
                    }
                }
            }

            // Wait for a defined interval before running the next cleanup check
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
    public void Dispose()
    {
        _cancellationTokenSource.Cancel(); // Stop the background task when disposing
        try
        {
            _cleanUpDictionaries.Wait();
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Exception in cleanup task: {ex.Message}");
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }
}