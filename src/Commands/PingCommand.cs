using System.Net.Sockets;

namespace codecrafters_redis.Commands;

public class PingCommand(Socket socket) : IRedisCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Send the PONG response using the RESP parser for simple strings
            await socket.SendAsync(RespResponseParser.ParseRespString("PONG"),
                SocketFlags.None, cancellationToken );
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