using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class UnaryEncoder : IUnaryEncoder
{
    private readonly IFileHandler _fileHandler;

    public UnaryEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }
    
    public void Encode(byte[] bytes, int divisor)
    {
        var bit = 0;
        var encodedBytes = new List<string>();
        BuildHeader(encodedBytes, divisor);

        foreach (var item in bytes)
        {
            if (bit == 1)
            {
                encodedBytes.Add(AddOne(item));
                bit = 0;
            }
            else
            { 
                encodedBytes.Add(AddZero(item));
                bit = 1;
            }
        }
        var list = StringUtils
            .Split8BitsList(encodedBytes)
            .Select(x => Convert.ToByte(x, 2))
            .ToArray();
        
        _fileHandler.Write(list, OperationType.Encode);
    }

    public void Decode(byte[] bytes, int divisor)
    {
        var convertedArray = bytes.Select(x => 
            Convert.ToString(x, 2)
                .PadLeft(8, '0'))
            .ToArray();
        
        var decoded = new List<byte>();
        var str = "";
        
        for (var i = 0; i < convertedArray.Length; i++)
        {
            str += convertedArray[i];
            
            if (convertedArray[i] != convertedArray[i + 1])
            {
                str += convertedArray[i + 1];
                var countValues = BytesUtils.Count(str);
                decoded.Add(countValues);
                str = "";
                i++;
            }
        }

        _fileHandler.Write(decoded.ToArray(), OperationType.Decode);
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000011"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private static string AddZero(byte b)
    {
        var remainder = CalculateRemainder(b);
        var total = remainder + b;
        var valueWithZero = string.Empty.PadRight(b, '0');
        
        return valueWithZero.PadRight(total, '1');
    }
    
    private static string AddOne(byte b)
    {
        var remainder = CalculateRemainder(b);
        var total = remainder + b;
        var valueWithOne = string.Empty.PadRight(b, '1');
        
        return valueWithOne.PadRight(total, '0');
    }

    private static int CalculateRemainder(byte b)
    {
        var remainder = b % 8;
        return 8 - remainder;
    }
}