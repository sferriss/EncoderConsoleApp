namespace EncoderConsoleApp.Handlers.Encoders.Interfaces;

public interface IEncoder
{
    void Encode(byte[] bytes, int divisor);
    
    void Decode(byte[] bytes, int divisor);

    void BuildHeader(IList<string> list, int divider);
}