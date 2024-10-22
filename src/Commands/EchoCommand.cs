using System.Net.Sockets;

namespace codecrafters_redis.Commands;

public class EchoCommand(Socket socket, string[] args) : IRedisCommand
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

            await socket.SendAsync(RespResponseParser.ParseRespString(args[0]), 
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