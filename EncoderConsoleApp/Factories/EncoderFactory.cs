using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;

namespace EncoderConsoleApp.Factories;

internal class EncoderFactory : IEncoderFactory
{
    private readonly IUnaryEncoder _unaryEncoder;
    private readonly IDeltaEncoder _deltaEncoder;
    private readonly IEliasGammaEncoder _eliasGammaEncoder;
    private readonly IFibonacciEncoder _fibonacciEncoder;
    private readonly IGolombEncoder _golombEncoder;

    public EncoderFactory(
        IUnaryEncoder unaryEncoder, 
        IDeltaEncoder deltaEncoder, 
        IEliasGammaEncoder eliasGammaEncoder,
        IFibonacciEncoder fibonacciEncoder,
        IGolombEncoder golombEncoder)
    {
        _unaryEncoder = unaryEncoder;
        _deltaEncoder = deltaEncoder;
        _eliasGammaEncoder = eliasGammaEncoder;
        _fibonacciEncoder = fibonacciEncoder;
        _golombEncoder = golombEncoder;
    }

    public IEncoder Build(EncoderType type) => type switch
    {
        EncoderType.Delta => _deltaEncoder,
        EncoderType.Fibonacci => _fibonacciEncoder,
        EncoderType.Golomb => _golombEncoder,
        EncoderType.Unary => _unaryEncoder,
        EncoderType.EliasGamma => _eliasGammaEncoder,
        
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid encoder")
    };
}