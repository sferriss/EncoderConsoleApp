using EncoderConsoleApp.Enums;

namespace EncoderConsoleApp.Handlers.Files;

public class FileHandler : IFileHandler
{
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
        File.WriteAllBytes(
            OperationType.Decode == type
                ? @$"..\..\..\ReturnedFiles\Decoded\decode.txt"
                : @"..\..\..\ReturnedFiles\Encoded\encoded.code", bytes);
    }
}