using System.Net.Sockets;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis.Commands;

public class GetCommand(Socket socket, string[] args, IKeyValueRepository keyValueRepository) : IRedisCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (args.Length != 1)
            {
                await socket.SendAsync(RespResponseParser.ParseRespError("Not valid number of arguments"),
                    SocketFlags.None, cancellationToken);
                return;
            }
            var result = await keyValueRepository.GetAsync(args[0]);
            await socket.SendAsync(RespResponseParser.ParseRespBulkString(result!),
                SocketFlags.None, cancellationToken);
            return;
        }
        catch (SocketException ex)
        {
            // Handle socket-specific errors, e.g., broken connections
            Console.WriteLine($"SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle any general exceptions
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}