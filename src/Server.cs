using System.Net;
using System.Net.Sockets;
using codecrafters_redis.Commands;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 6379); //Port 6379 is the default port for Redis
        server.Start();
        using IKeyValueRepository keyValueRepository = new InMemoryKeyValueRepository();
        while (true)
        {
            var socket = await server.AcceptSocketAsync(); // wait for client 
            System.Console.WriteLine("Client connected");
            _ = Task.Run(async () =>
            {
                try
                {
                    while (socket.Connected)
                    {
                        var buffer = new byte[1_024];
                        System.Console.WriteLine("Receiving Request");
                        int bytesReceived = await socket.ReceiveAsync(buffer, SocketFlags.None);
                        System.Console.WriteLine("Request received");
                        if (bytesReceived == 0)
                        {
                            System.Console.WriteLine("Connection closed");
                            break; //Connection closed
                        }

                        var (command, arguments) = RespRequestParser.ParseRequest(buffer[..bytesReceived]);
                        await HandleRedisCommand(command, arguments, socket,keyValueRepository );
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Exception : " + e.Message);
                }
                finally
                {
                    if (socket.Connected)
                    {
                        System.Console.WriteLine("ShutingDown the connection");
                        socket.Shutdown(SocketShutdown.Both);
                    }

                    System.Console.WriteLine("Closing the connection");
                    socket.Close();
                }
            });
        }
    }

    private static async Task HandleRedisCommand(string command, string[] arguments, Socket socket, IKeyValueRepository keyValueRepository)
    {
        Console.WriteLine($"Command : {command}");
        Console.WriteLine($"Arguments : {string.Join(',', arguments)}");
        switch (command.ToUpper())
        {
            case "PING":
                await new PingCommand(socket).ExecuteAsync();
                break;
            case "ECHO":
                await new EchoCommand(socket, arguments).ExecuteAsync();
                break;
            case "SET":
                await new SetCommand(socket, arguments, keyValueRepository)
                    .ExecuteAsync();
                break;
            case "GET" :
                await new GetCommand(socket, arguments, keyValueRepository)
                    .ExecuteAsync();
                break;
            default:
                await socket.SendAsync(RespResponseParser.ParseRespError("ERR unknown command"),
                    SocketFlags.None);
                break;
        }
    }
}