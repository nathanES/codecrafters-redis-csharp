using System.Net.Sockets;
using codecrafters_redis.Common;
using codecrafters_redis.Configuration;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis.Commands;

public class CommandHandler
{
    private readonly Socket _socket;
    private readonly List<IRepository> _repositories;
    private readonly List<IService> _services;

    public CommandHandler(Socket socket, List<IRepository> repositories, List<IService> services)
    {
        _socket = socket;
        _repositories = repositories;
        _services = services;
    }

    public async Task HandleCommand(string command, string[] arguments)
    {
        Console.WriteLine($"Command : {command}");
        Console.WriteLine($"Arguments : {string.Join(',', arguments)}");
        switch (command.ToUpper())
        {
            case "PING":
                await new PingCommand(_socket).ExecuteAsync();
                break;
            case "ECHO":
                if (arguments.Length < 1)
                {
                    await _socket.SendAsync(RespResponseParser.ParseRespError("ERR wrong number of arguments for 'ECHO' command"),
                        SocketFlags.None);
                    return;
                }
                await new EchoCommand(_socket, arguments).ExecuteAsync();

                break;
            case "SET":
                if (arguments.Length < 2)
                {
                    await _socket.SendAsync(RespResponseParser.ParseRespError("ERR wrong number of arguments for 'SET' command"),
                        SocketFlags.None);
                    return;
                }
                await new SetCommand(_socket, arguments,
                        (IKeyValueRepository)_repositories.First(x => x is IKeyValueRepository))
                    .ExecuteAsync();
                break;
            case "GET":
                if (arguments.Length != 1)
                {
                    await _socket.SendAsync(RespResponseParser.ParseRespError("ERR wrong number of arguments for 'GET' command"),
                        SocketFlags.None);
                    return;
                }

                await new GetCommand(_socket, arguments,
                        (IKeyValueRepository)_repositories.First(x => x is IKeyValueRepository))
                    .ExecuteAsync();
                break;
            case "CONFIG":
                if (arguments.Length < 1)
                {
                    await _socket.SendAsync(RespResponseParser.ParseRespError("ERR wrong number of arguments for 'CONFIG' command"),
                        SocketFlags.None);
                    return;
                }
                await HandleConfigCommand(arguments[0], arguments[1..]);
                break;
            default:
                await _socket.SendAsync(RespResponseParser.ParseRespError("ERR unknown command"),
                    SocketFlags.None);
                break;
        }
    }

    private async Task HandleConfigCommand(string command, string[] arguments)
    {
        switch (command.ToUpper())
        {
            case "GET":
                var configService = (ConfigService)_services.First(x => x is ConfigService);
                await new GetConfigCommand(_socket, arguments, configService).ExecuteAsync();
                break;
        }
    }
}