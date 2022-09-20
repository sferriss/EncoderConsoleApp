using EncoderConsoleApp.Enums;

namespace EncoderConsoleApp.Handlers.Files;

public class FileHandler : IFileHandler
{
    private string? _bitsStringControle;

    public byte[] Read(string path, OperationType type)
    {
        var data = File.ReadAllBytes(path);

        return type == OperationType.Encode ? data : data[2..];
    }
    
    public byte[] ReadHeader(string path)
    {
        var data = File.ReadAllBytes(path);

        return data[..2];
    }

    public void Write(byte[] bytes, OperationType type)
    {
        if (OperationType.Decode == type)
        {
            File.WriteAllBytes(@$"..\..\..\ReturnedFiles\Decoded\decode.txt", bytes);
        }
        else
        {
            File.WriteAllBytes(@"..\..\..\ReturnedFiles\Encoded\encoded.code", bytes);
        }
    }
}