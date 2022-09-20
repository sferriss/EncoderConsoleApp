﻿using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;

namespace EncoderConsoleApp.Handlers.Encoders;

public class EliasGammaEncoder : IEliasGammaEncoder
{
    private readonly IFileHandler _fileHandler;

    public EliasGammaEncoder(IFileHandler fileHandler)
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
        const string firstByte = "00000001"; //Algorithm type
        const string secondByte = "00000000"; // Golomb divider
        
        list.Add(firstByte);
        list.Add(secondByte);
    }
}