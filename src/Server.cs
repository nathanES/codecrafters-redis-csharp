using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        // You can use print statements as follows for debugging, they'll be visible when running tests.
        Console.WriteLine("Logs from your program will appear here!");

        TcpListener server = new TcpListener(IPAddress.Any, 6379);//Port 6379 is the default port for Redis
        server.Start();
        var sockets = server.AcceptSocket(); // wait for client 
        sockets.SendAsync(FormatResponse("PONG"));
        server.Stop();

        byte[] FormatResponse(string response)
        {
            return Encoding.UTF8.GetBytes($"+{response}\r\n");
        }
    }
}