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
            var sockets = server.AcceptSocket(); // wait for client 
            while(sockets.Connected)
            {
                var buffer = new byte[1_024];
                await sockets.ReceiveAsync(buffer);
                await sockets.SendAsync(FormatResponse("PONG"));
            }
        }

        byte[] FormatResponse(string response)
        {
            return Encoding.UTF8.GetBytes($"+{response}\r\n");
        }
    }
}