using System.Text;

namespace codecrafters_redis;

public static class RespParser
{
    //Example of commands : *2\r\n$4\r\nECHO\r\n$5\r\nhello\r\n
    public static (string Command, string[] Args) ParseRequest(byte[] requestAsBytes)
    {
        int index = 0;
        if (requestAsBytes[index] != '*')
            throw new ArgumentException("InvalidRESP: Does not start with '*'");

        index++;
        int numberOfArgs = ParseInteger(requestAsBytes, ref index);

        string[] args = new string[numberOfArgs - 1];

        string command = ParseRespBulkString(requestAsBytes, ref index); //Command is the first bulk string

        for (int i = 0; i < numberOfArgs - 1; i++)
        {
            args[i] = ParseRespBulkString(requestAsBytes, ref index);
        }

        return (command, args);
    }

    private static int ParseInteger(byte[] data, ref int index)
    {
        int result = 0;
        bool negative = false;

        if (data[index] == '-')
        {
            negative = true;
            index++;
        }

        while (data[index] != '\r')
        {
            result = result * 10 + (data[index] - '0');
            index++;
        }

        index += 2; //skip \r\n

        return negative ? -1 * result : result;
    }

    private static string ParseRespBulkString(byte[] data, ref int index)
    {
        if (data[index] != '$')
            throw new ArgumentException("Invalid Bulk String: does not start with $");

        index++;
        int length = ParseInteger(data, ref index);

        if (length == -1)
            return null; //Null bulk string

        string result = Encoding.UTF8.GetString(data, index, length);
        index += length + 2; //skip over string data and \r\n

        return result;
    }
}