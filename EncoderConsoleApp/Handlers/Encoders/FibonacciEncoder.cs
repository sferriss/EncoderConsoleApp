using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class FibonacciEncoder : IFibonacciEncoder
{
    private readonly IFileHandler _fileHandler;

    public FibonacciEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Encode(byte[] bytes, int divisor)
    {
        var fibonacci = GenerateFibonacciSequence();
        var bytesWithSum = BytesUtils.AddOneInEachByte(bytes);

        var bitsList = new List<string>();
        BuildHeader(bitsList, 0);
        
        var codeword = "";

        foreach (var t in bytesWithSum)
        {
            var fibonacciFilter = fibonacci.Where(x => x <= t).ToArray();
            var str = "1";
            var sum = 0;
            
            foreach (var item in fibonacciFilter)
            {
                if (sum + item <= t)
                {
                    str += "1";
                    sum += item;
                    
                    continue;
                }
                
                str += "0";
            }
            
            var strReverse = new string(str.Reverse().ToArray());
            codeword += strReverse;

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
        var fibonacci = GenerateFibonacciSequence();

        var convertedArray = bytes.Select(x => 
                Convert.ToString(x, 2)
                    .PadLeft(8, '0'))
            .ToArray();
        
        var bytesList = new List<byte>();
        var predecessor = ' ';
        var arrayInt = new List<int>();
        var count = 0;

        foreach (var item in convertedArray)
        {
            var arrayChar = item.ToArray();

            foreach (var t in arrayChar)
            {
                if (t == '1' && t == predecessor && count >= 5)
                {
                    var calculatedByte = CompareWithFibonacciSequence(fibonacci, arrayInt.ToArray());
                    bytesList.Add(calculatedByte);
                    arrayInt = new List<int>();
                    predecessor = ' ';
                    count = 0;
                }
                else
                {
                    arrayInt.Add(int.Parse(t.ToString()));
                    predecessor = t;
                    count++;
                }
            }
        }

        var bytesWithSub = BytesUtils.SubOneInEachByte(bytesList.ToArray());
        _fileHandler.Write(bytesWithSub, OperationType.Decode);
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000010"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private static int[] GenerateFibonacciSequence()
    {
        var fibonacciArray = new int[20];
        fibonacciArray[0] = 1;
        fibonacciArray[1] = 2;
        
        for (var i = 2; i < fibonacciArray.Length; i++) {
            fibonacciArray[i] = fibonacciArray[i - 1] + fibonacciArray[i - 2];
        }
        return fibonacciArray.Reverse().ToArray();
    }

    private static byte CompareWithFibonacciSequence(int[] fibonacci, int[] arrayToCompare)
    {
        var fibonacciReverse = fibonacci.Reverse().ToArray();
        var sum = 0;
        for (var i = 0; i < arrayToCompare.Length; i++)
        {
            if (arrayToCompare[i] == 1)
            {
                sum += fibonacciReverse[i];
            }
        }
        
        return (byte) sum;
    }
}