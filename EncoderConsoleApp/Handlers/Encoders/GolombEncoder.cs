using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class GolombEncoder : IGolombEncoder
{
    private readonly IFileHandler _fileHandler;
    private const int StopBit = 1;

    public GolombEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Encode(byte[] bytes, int divider)
    {
        var bytesAddedOne = BytesUtils.AddOneInEachByte(bytes);
        
        var bitsList = new List<string>();
        BuildHeader(bitsList, divider);

        foreach (var item in bytesAddedOne)
        {
            var (dividend, remainder) = GetDividendAndRemainder(item, divider);
            var prefix = string.Empty.PadLeft(dividend, '0');
            var suffix = Convert.ToString(remainder, 2);

            var codeword = prefix + StopBit;
            

            var remainder8Bits = codeword.Length % 8;
            if (remainder8Bits != 0)
            {
                var total = (8 - remainder8Bits) + codeword.Length;
                codeword = codeword.PadRight(total, '0');
            }
            
            suffix = suffix.PadLeft(8, '0');
            codeword += suffix;
            
            bitsList.AddRange(StringUtils.Split8BitsList(codeword));
        }
        
        var byteArray = bitsList
            .Select(x => Convert.ToByte(x, 2))
            .ToArray();

        _fileHandler.Write(byteArray, OperationType.Encode);
    }

    public void Decode(byte[] bytes, int divisor)
    {
        var bytesList = new List<byte>();

        var convertedArray = bytes.Select(x => 
                Convert.ToString(x, 2)
                    .PadLeft(8, '0'))
            .ToArray();

        var foundStopBit = false;
        var codeword = "";
        var prefixCounter = 0;

        foreach (var item in convertedArray)
        {
            var arrayChar = item.ToArray();

            for (var i = 0; i < arrayChar.Length; i++)
            {
                if (arrayChar[i] == '1' && !foundStopBit)
                {
                    foundStopBit = true;
                    break;
                }

                if (foundStopBit)
                {
                    codeword += arrayChar[i];
                }

                if (!foundStopBit)
                {
                    prefixCounter++;   
                }
                
                if (foundStopBit && i == arrayChar.Length - 1)
                {
                    var byteValue = CountValue(prefixCounter, codeword, divisor);
                    bytesList.Add(byteValue);

                    foundStopBit = false;
                    prefixCounter = 0;
                    codeword = "";
                }
            }
        }
        
        var bytesWithSub = BytesUtils.SubOneInEachByte(bytesList.ToArray());
        
        _fileHandler.Write(bytesWithSub, OperationType.Decode);
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000000"; //Algorithm type
        var secondByte = BytesUtils.IntegerToStringBinary(divider, 8); // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private static (int, int) GetDividendAndRemainder(int value, int divider)
    {
        var dividend = value / divider;
        var remainder = value % divider;

        return (dividend, remainder);
    }

    private static byte CountValue(int prefixCount, string suffix, int divider)
    {
        var suffixValue = Convert.ToInt32(suffix, 2);

        var total = (divider * prefixCount) + suffixValue;

        return (byte)total;
    }
}