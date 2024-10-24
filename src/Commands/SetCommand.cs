using System.Net.Sockets;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis.Commands;

public class SetCommand(Socket socket, string[] args, IKeyValueRepository keyValueRepository) : IRedisCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string key = args[0];
            string value = args[1];

            int? expiryTimeMilliseconds = null;
            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "PX":
                        if (args.Length < i + 1)
                        {
                            await socket.SendAsync(RespResponseParser.ParseRespError("Expiry is missing"),
                                SocketFlags.None, cancellationToken);
                            return;
                        }

                        if (!int.TryParse(args[++i], out var expiry))
                        {
                            await socket.SendAsync(RespResponseParser.ParseRespError("Expiry is not a number"),
                                SocketFlags.None, cancellationToken);
                            return;
                        }

                        expiryTimeMilliseconds = expiry;
                        break;
                    default:
                        break;
                }
            }

            await keyValueRepository.SetAsync(args[0], args[1], expiryTimeMilliseconds);

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