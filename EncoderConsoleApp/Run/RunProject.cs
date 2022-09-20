using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Factories;
using EncoderConsoleApp.Handlers.Files;

namespace EncoderConsoleApp.Run;

public class RunProject
{
    private readonly IEncoderFactory _factory;
    private readonly IFileHandler _fileHandler;

    public RunProject(IEncoderFactory factory, IFileHandler fileHandler)
    {
        _factory = factory;
        _fileHandler = fileHandler;
    }

    public void Run()
    {
        var option = "";
        var divider = 0;
        
        while (option != "5")
        {
            option = CallMainMenu();

            if (option == "1")
            {
                option = CallEncodersMenu();
                var encoderType = GetTypeByValue(int.Parse(option));
                var decoder = _factory.Build(encoderType);

                if (encoderType == EncoderType.Golomb)
                {
                    option = CallDiverMenu();
                    divider = int.Parse(option);
                }
                
                option = CallPathMenu();
                var file = _fileHandler.Read(option, OperationType.Encode);
                decoder.Encode(file, divider);
                Console.Write("\nCodificado... \n");
            }
            else if(option == "2")
            {
                option = CallPathMenu();
                var header = _fileHandler.ReadHeader(option);
                var file = _fileHandler.Read(option, OperationType.Decode);
                
                var type = header[0];
                divider = header[1];

                var decoder = _factory.Build(GetTypeByValue(type));
                decoder.Decode(file, divider);
                Console.Write("\nDecodificando... \n");
            }
            else if(option == "5")
            {
                Console.Write("Encerrando \n");
            }
            else
            {
                Console.Write("Opção inválida! \n");
            }
        }
    }

    private string? CallMainMenu()
    {
        Console.Write("\nSelecione uma opção: \n" +
                      "1 - Condificar \n" +
                      "2 - Decodificar \n" +
                      "5 - Sair \n");

        return Console.ReadLine();
    }
    
    private string? CallEncodersMenu()
    {
        Console.Write("\nSelecione um codificador: \n" +
                      "0 - Golomb \n" +
                      "1 - Elias-Gamma \n" +
                      "2 - Fibonacci \n" +
                      "3 - Unary \n" +
                      "4 - Delta \n");

        return Console.ReadLine();
    }
    
    private string? CallPathMenu()
    {
        Console.Write("\nInforme o caminho do arquivo: \n");

        return Console.ReadLine();
    }
    
    private string? CallDiverMenu()
    {
        Console.Write("\nInforme o divisor: \n");

        return Console.ReadLine();
    }
    
    private static EncoderType GetTypeByValue(int value) => value switch
    {
        0 => EncoderType.Golomb,
        1 => EncoderType.EliasGamma,
        2 => EncoderType.Fibonacci,
        3 => EncoderType.Unary,
        4 => EncoderType.Delta,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Encoder type not found")
    };
}