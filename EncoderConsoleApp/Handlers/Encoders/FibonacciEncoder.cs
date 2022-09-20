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
        }

        var fullList = StringUtils
            .Split8BitsList(strList)
            .Select(x => Convert.ToByte(x, 2)).ToList();
            
        var list = StringUtils
            .Split8BitsList(fullStr)
            .Select(x => Convert.ToByte(x, 2))
            .ToList();
        
        fullList.AddRange(list);
        
        _fileHandler.Write(fullList.ToArray(), OperationType.Encode);
    }

    public void Decode(byte[] bytes, int divisor)
    {
        throw new NotImplementedException();
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000010"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private int[] GenerateFibonacciSequence()
    {
        var fibonacciArray = new int[12];
        fibonacciArray[0] = 1;
        fibonacciArray[1] = 2;
        
        for (int i = 2; i < fibonacciArray.Length; i++) {
            fibonacciArray[i] = fibonacciArray[i - 1] + fibonacciArray[i - 2];
        }
        return fibonacciArray.Reverse().ToArray();
    }
}