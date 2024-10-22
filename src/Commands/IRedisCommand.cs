namespace codecrafters_redis.Commands;

internal interface IRedisCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}