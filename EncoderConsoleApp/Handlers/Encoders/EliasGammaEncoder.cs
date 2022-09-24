using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class EliasGammaEncoder : IEliasGammaEncoder
{
    private readonly IFileHandler _fileHandler;
    private const int StopBit = 1;

    public EliasGammaEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Encode(byte[] bytes, int divisor)
    {
        var potencyValues = CalculatePotency();
        var bytesAddedOne = BytesUtils.AddOneInEachByte(bytes);

        var bytesList = new List<string>();
        BuildHeader(bytesList, 0);

        var codeword = "";

        foreach (var item in bytesAddedOne)
        {
            var (exponent, potency) = GetPotencyValue(potencyValues, item);
            var remainder = CalculateDifferent(item, potency);

            var prefix = string.Empty.PadLeft(exponent, '0');
            var suffix = Convert.ToString(remainder, 2).PadLeft(exponent, '0');
            codeword += prefix + StopBit + suffix;
            
            if (codeword.Length < 8) continue;
            
            bytesList.Add(codeword[..8]);
            codeword = codeword[8..];
        }
        bytesList.AddRange(StringUtils.Split8BitsList(codeword));
        
        var byteArray = bytesList
            .Select(x => Convert.ToByte(x, 2))
            .ToArray();

        _fileHandler.Write(byteArray, OperationType.Encode);
    }

    public void Decode(byte[] bytes, int divisor)
    {
        var bytesList = new List<byte>();
        var potencyValues = CalculatePotency();
        
        var convertedArray = bytes.Select(x => 
                Convert.ToString(x, 2)
                    .PadLeft(8, '0'))
            .ToArray();

        var countSuffix = 0;
        var foundStopBit = false;
        var codeword = "";
        
        foreach (var item in convertedArray)
        {
            var arrayChar = item.ToArray();

            foreach (var c in arrayChar)
            {
                if (c == '1' && !foundStopBit)
                {
                    foundStopBit = true;
                    continue;
                }

                switch (foundStopBit)
                {
                    case true when codeword.Length < countSuffix:
                        codeword += c;
                        continue;
                    case true when codeword.Length == countSuffix:
                        var byteDecoded = GetByteDecoded(potencyValues, codeword, countSuffix);
                        bytesList.Add(byteDecoded);
                        foundStopBit = false;
                        countSuffix = 0;
                        codeword = "";
                        break;
                }

                countSuffix++;
            }
        }

        var bytesWithSub = BytesUtils.SubOneInEachByte(bytesList.ToArray());
        
        _fileHandler.Write(bytesWithSub, OperationType.Decode);
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000001"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }

    private static (int, int)[] CalculatePotency()
    {
        var array = new (int, int) [15];
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = (i, (int)Math.Pow(2, i));
        }

        return array.Reverse().ToArray();
    }

    private static int CalculateDifferent(int ascValue, int potencyValue)
    {
        return ascValue - potencyValue;
    }
    
    private static (int, int) GetPotencyValue(IList<(int, int)> potencyArray, int valueToFind)
    {
        for (var i = 0; i < potencyArray.Count; i++)
        {
            if (potencyArray[i].Item2 > valueToFind) continue;

            return (potencyArray[i].Item1, potencyArray[i].Item2);
        }
        
        return (0, 0);
    }
    
    private static byte GetByteDecoded(IEnumerable<(int, int)> potencyArray, string codeword, int potency)
    {
        var remainder = Convert.ToInt32(codeword, 2);
        var potencyValue = potencyArray.First(x => x.Item1 == potency).Item2;
        
        return (byte)(remainder + potencyValue);
    }
}