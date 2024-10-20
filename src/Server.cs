using System.Net;
using System.Net.Sockets;
using System.Text;

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
    
                        System.Console.WriteLine("Sending Response");
                        await socket.SendAsync(FormatResponse("PONG"),SocketFlags.None );
                        System.Console.WriteLine("Response sent");
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