using System.Text;

namespace codecrafters_redis;

public static class RespResponseParser
{
    // For Simple String responses: +<message>\r\n
    public static byte[] ParseRespString(string response)
    {
        return Encoding.UTF8.GetBytes($"+{response}\r\n");
    }

    // For Error responses: -<error_message>\r\n
    public static byte[] ParseRespError(string response)
    {
        return Encoding.UTF8.GetBytes($"-{response}\r\n");
    }

    // For Integer responses: :<number>\r\n
    public static byte[] ParseRespInteger(int response)
    {
        return Encoding.UTF8.GetBytes($":{response}\r\n");
    }

    // For Integer responses as string input
    public static byte[] ParseRespInteger(string response)
    {
        return Encoding.UTF8.GetBytes($":{response}\r\n");
    }

    // For Bulk Strings: $<length>\r\n<string>\r\n
    public static byte[] ParseRespBulkString(string response)
    {
        if (response is null)
        {
            return Encoding.UTF8.GetBytes("$-1\r\n"); // Null bulk string
        }

        return Encoding.UTF8.GetBytes($"${response.Length}\r\n{response}\r\n");
    }

    // For Arrays: *<number of elements>\r\n followed by elements in RESP format
    public static byte[] ParseRespArray(string[] responses)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"*{responses.Length}\r\n");

        foreach (var response in responses)
        {
            sb.Append($"${response.Length}\r\n{response}\r\n");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}