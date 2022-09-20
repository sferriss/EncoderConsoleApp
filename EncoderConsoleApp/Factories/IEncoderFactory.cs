using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;

namespace EncoderConsoleApp.Factories;

public interface IEncoderFactory
{
    IEncoder Build(EncoderType type);
}