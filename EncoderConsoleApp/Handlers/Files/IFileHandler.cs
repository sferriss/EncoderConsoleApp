using EncoderConsoleApp.Enums;

namespace EncoderConsoleApp.Handlers.Files;

public interface IFileHandler
{
    byte[] Read(string path, OperationType type);

    byte[] ReadHeader(string path);
    
    void  Write(byte[] bytes, OperationType type);
}