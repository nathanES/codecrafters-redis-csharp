using System.Net;
using System.Net.Sockets;
using codecrafters_redis.Commands;
using codecrafters_redis.Common;
using codecrafters_redis.Configuration;
using codecrafters_redis.KeyValue;

namespace codecrafters_redis;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 6379); //Port 6379 is the default port for Redis
        server.Start();
       
        using IKeyValueRepository keyValueRepository = new InMemoryKeyValueRepository();//Using to stop the backThread
        IConfigRepository configRepository = new InMemoryConfigRepository();
        List<IRepository> repositories = new List<IRepository>(){keyValueRepository};

        ConfigService configService = new ConfigService(configRepository);
        List<IService> services = new List<IService>(){configService};

        await configService.SetAsync(args);

        try
        {
            while (true)
            {
                var socket = await server.AcceptSocketAsync(); // wait for client 
                Console.WriteLine("Client connected");
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (socket.Connected)
                        {
                            var buffer = new byte[1_024];
                            Console.WriteLine("Receiving Request");
                            int bytesReceived = await socket.ReceiveAsync(buffer, SocketFlags.None);
                            Console.WriteLine("Request received");
                            if (bytesReceived == 0)
                            {
                                Console.WriteLine("Connection closed");
                                break; //Connection closed
                            }

                            var (command, arguments) = RespRequestParser.ParseRequest(buffer[..bytesReceived]);
                            await new CommandHandler(socket, repositories, services).HandleCommand(command, arguments);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                    finally
                    {
                        if (socket.Connected)
                        {
                            Console.WriteLine("ShutingDown the connection");
                            socket.Shutdown(SocketShutdown.Both);
                        }

                        Console.WriteLine("Closing the connection");
                        socket.Close();
                    }
                });
            }
        }
        finally
        {
            server.Stop();
            System.Console.WriteLine("Server stopped.");
        }
    }
}