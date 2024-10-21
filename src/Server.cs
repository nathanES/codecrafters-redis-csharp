using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 6379);//Port 6379 is the default port for Redis
        server.Start();
        while (true)
        {
            var socket = await server.AcceptSocketAsync(); // wait for client 
            System.Console.WriteLine("Client connected");
            _ = Task.Run(async() =>
            {
                try
                {
                    while(socket.Connected)
                    {
                        var buffer = new byte[1_024];
                        System.Console.WriteLine("Receiving Request");
                        int bytesReceived = await socket.ReceiveAsync(buffer, SocketFlags.None);
                        System.Console.WriteLine("Request received");
                        if(bytesReceived == 0)
                        {
                            System.Console.WriteLine("Connection closed");
                            break; //Connection closed
                        }

                        var (command, arguments) = RespParser.ParseRequest(buffer[..bytesReceived]);
                        switch (command.ToUpper())
                        {
                            case "PING":
                                await socket.SendAsync(FormatResponse("PONG"),SocketFlags.None );
                                System.Console.WriteLine("Response sent");
                                break;
                                case "ECHO" :
                                await socket.SendAsync(FormatResponse(arguments[0]),SocketFlags.None );
                                System.Console.WriteLine("Response sent");
                                    break;
                            default:
                                throw new NotImplementedException("Command not handled");
                        }
                        
                        
                    }
                }
                catch(Exception e)
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
  



        byte[] FormatResponse(string response)
        {
            return Encoding.UTF8.GetBytes($"+{response}\r\n");
        }
    }
}