using System.Net.Sockets;
using System.Text;
using codecrafters_redis.Configuration;

namespace codecrafters_redis.Commands;

public class GetConfigCommand(Socket socket, string[] args, ConfigService service) : IRedisCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string key = args[0];
            string? result = await service.Get(key);
            if (result is null)
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes("$-1\r\n"),SocketFlags.None, cancellationToken);
                return;
            }
            await socket.SendAsync(RespResponseParser.ParseRespArray(new string[]{key, result}),
                SocketFlags.None, cancellationToken);  
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