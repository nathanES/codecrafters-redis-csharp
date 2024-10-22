using System.Net.Sockets;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis.Commands;

public class SetCommand(Socket socket, string[] args, IKeyValueRepository keyValueRepository)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (args.Length < 2)
            {
                await socket.SendAsync(RespResponseParser.ParseRespError("Not valid number of arguments"),
                    SocketFlags.None, cancellationToken);
                return;
            }
            await keyValueRepository.SetAsync(args[0], args[1]);
            await socket.SendAsync(RespResponseParser.ParseRespString("OK"),
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