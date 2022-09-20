using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Utils;

namespace EncoderConsoleApp.Handlers.Encoders;

public class GolombEncoder : IGolombEncoder
{
    private readonly IFileHandler _fileHandler;

    public GolombEncoder(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Encode(byte[] bytes, int divisor)
    {
        throw new NotImplementedException();
    }

    public void Decode(byte[] bytes, int divisor)
    {
        throw new NotImplementedException();
    }

    public void BuildHeader(IList<string> list, int divider)
    {
        const string firstByte = "00000000"; //Algorithm type
        var secondByte = BytesUtils.IntegerToStringBinary(divider, 8); // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }
}