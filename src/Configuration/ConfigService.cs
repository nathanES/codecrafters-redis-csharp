using codecrafters_redis.Common;

namespace codecrafters_redis.Configuration;

public class ConfigService :IService
{
    private readonly IConfigRepository _repository;
    private readonly Dictionary<string, Func<string[], int, Task>> _configHandlers;

    public ConfigService(IConfigRepository repository)
    {
        _repository = repository;
        _configHandlers = new Dictionary<string, Func<string[], int, Task>>
        {
            { "--dir", async (args, index) => await HandleConfigParameter(args, index, "dir") },
            { "--dbfilename", async (args, index) => await HandleConfigParameter(args, index, "dbfilename") }
        };
    }

    public async Task SetAsync(string[] arguments)
    {
        int count = arguments.Length;
        for (int i = 0; i < count; i++)
        {
            if (_configHandlers.TryGetValue(arguments[i], out var handler))
            {
                await handler(arguments, i);
                i++; // Skip the next argument as it will be the value for the current config
            }
        }
    }

    public async Task<string?> Get(string key)
    {
        return await _repository.GetAsync(key);
    }
    private async Task HandleConfigParameter(string[] arguments, int index, string configKey)
    {
        if (++index >= arguments.Length)
        {
            Console.WriteLine($"Impossible to add {configKey} in config");
            return;
        }
        await _repository.SetAsync(configKey, arguments[index]);
    }


}