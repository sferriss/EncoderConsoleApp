using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class DeltaEncoder : IDeltaEncoder
{
    private readonly IFileHandler _fileHandler;

    public DeltaEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Encode(byte[] bytes, int divider)
    {
        var bytesAddedOne = BytesUtils.AddOneInEachByte(bytes);
        
        var bitsList = new List<string>();
        BuildHeader(bitsList, 0);

        var codeword = "";

        for (var i = 0; i < bytesAddedOne.Length - 1; i++)
        {
            if (i == 0)
            {
                var firstValue =Convert.ToString(bytesAddedOne[i], 2).PadLeft(8, '0');
                bitsList.Add(firstValue);
            }
            
            if (bytesAddedOne[i] != bytesAddedOne[i + 1])
            {
                codeword += '1';
                var value = bytesAddedOne[i + 1] - bytesAddedOne[i];
                
                if (value > 0)
                {
                    codeword += '0';
                }
                else
                {
                    codeword += '1';
                }

                var convertedDiff = Convert.ToString(Math.Abs(value), 2).PadLeft(8, '0');
                codeword += convertedDiff;
            }
            else
            {
                codeword += string.Empty.PadLeft(8, '0');
            }
            
            if (codeword.Length < 8) continue;
            
            bitsList.Add(codeword[..8]);
            codeword = codeword[8..];
        }
        
        bitsList.AddRange(StringUtils.Split8BitsList(codeword));
        
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

        var foundChange = false;
        var codeword = "";
        var count = 0;
        var isPositive = false;
        var startCount = false;

        for (var i = 0; i < convertedArray.Length; i++)
        {
            if (i == 0)
            {
                bytesList.Add(Convert.ToByte(convertedArray[i], 2));
                continue;
            }
            
            var arrayChar = convertedArray[i].ToArray();

            foreach (var c in arrayChar)
            {
                if (c == '1' && !foundChange)
                {
                    foundChange = true;
                    continue;
                }

                switch (foundChange)
                {
                    case true when c == '0' && !startCount:
                        isPositive = true;
                        startCount = true;
                        break;
                    case true when c == '1' && !startCount:
                        isPositive = false;
                        startCount = true;
                        break;
                    case true:
                        codeword += c;
                        count++;
                        break;
                    case false:
                        codeword += c;
                        count++;
                        break;
                }

                if (count != 8) continue;
                
                var convertedByte = GetConvertedByte(codeword, bytesList, isPositive);
                bytesList.Add(convertedByte);
                count = 0;
                codeword = "";
                isPositive = false;
                foundChange = false;
                startCount = false;
            }
        }
        
        var bytesWithSub = BytesUtils.SubOneInEachByte(bytesList.ToArray());
        
        _fileHandler.Write(bytesWithSub, OperationType.Decode);
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000100"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private static byte GetConvertedByte(string codeword, IEnumerable<byte> list, bool isPositive)
    {
        var convertedValue = Convert.ToInt32(codeword, 2);

        if (isPositive)
        {
            return (byte)(list.Last() + convertedValue);
        }
        
        return (byte)(list.Last() - convertedValue);
    }
}