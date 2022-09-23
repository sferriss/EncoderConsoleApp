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
        
        var strList = new List<string>();
        BuildHeader(strList, 0);
        
        var bytesList = new List<string>();
        
        var fullStr = "";

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
            fullStr += strReverse;

            if (fullStr.Length < 8) continue;
            
            bytesList.Add(fullStr[..8]);
            fullStr = fullStr[8..];
        }

        var fullList = StringUtils
            .Split8BitsList(strList)
            .Select(x => Convert.ToByte(x, 2)).ToList();
            
        var list = bytesList
            .Select(x => Convert.ToByte(x, 2))
            .ToList();
        
        fullList.AddRange(list);
        
        _fileHandler.Write(fullList.ToArray(), OperationType.Encode);
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

            for (var i = 0; i < arrayChar.Length; i++)
            {
                if (arrayChar[i] == '1' && arrayChar[i] == predecessor && count >= 5)
                {
                    var calculatedByte = CompareWithFibonacciSequence(fibonacci, arrayInt.ToArray());
                    bytesList.Add(calculatedByte);
                    arrayInt = new List<int>();
                    predecessor = ' ';
                    count = 0;
                }
                else
                {
                    arrayInt.Add(int.Parse(arrayChar[i].ToString()));
                    predecessor = arrayChar[i];
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
        var fibonacciArray = new int[10];
        fibonacciArray[0] = 1;
        fibonacciArray[1] = 2;
        
        for (int i = 2; i < fibonacciArray.Length; i++) {
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