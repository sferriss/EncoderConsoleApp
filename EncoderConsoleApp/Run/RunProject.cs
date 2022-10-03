using EncoderConsoleApp.Enums;
using EncoderConsoleApp.Factories;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.TratamentoRuido;

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
        
        while (option != "2")
        {
            option = CallMainMenu();

            switch (option)
            {
                case "0":
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
                    
                    Console.WriteLine("\nCodificando... \n");
                    decoder.Encode(file, divider);

                    Console.WriteLine(@"Salvo em: EncoderConsoleApp\EncoderConsoleApp\ReturnedFiles\Encoded\encoded.code");
                    break;
                }
                case "1":
                {
                    option = CallOptionsPathMenu();

                    switch (option)
                    {
                        case "0":
                        {
                            const string path = @"..\..\..\ReturnedFiles\Encoded\encoded.code";
                            var header = _fileHandler.ReadHeader(path);
                            var file = _fileHandler.Read(path, OperationType.Decode);
                
                            var type = header[0];
                            divider = header[1];

                            var decoder = _factory.Build(GetTypeByValue(type));
                            
                            Console.WriteLine("\nDecodificando... \n");
                            decoder.Decode(file, divider);
                            Console.WriteLine(@"Salvo em: EncoderConsoleApp\EncoderConsoleApp\ReturnedFiles\Decoded\decode.txt");
                            break;
                        }
                        case "1":
                        {
                            option = CallPathMenu();
                            var header = _fileHandler.ReadHeader(option);
                            var file = _fileHandler.Read(option, OperationType.Decode);
                
                            var type = header[0];
                            divider = header[1];

                            var decoder = _factory.Build(GetTypeByValue(type));
                            
                            Console.WriteLine("\nDecodificando... \n");
                            decoder.Decode(file, divider);
                            Console.WriteLine(@"Salvo em: EncoderConsoleApp\EncoderConsoleApp\ReturnedFiles\Decoded\decode.txt");
                            break;
                        }
                        default:
                            Console.WriteLine("Opção inválida! \n");
                            break;
                    }

                    break;
                }
                case "2":
                    Console.Clear();
                    Console.WriteLine("Encerrando \n");
                    break;
                default:
                    Console.WriteLine("Opção inválida! \n");
                    break;
            }
        }
    }

    private static string? CallMainMenu()
    {
        Console.Write("\nSelecione uma opção: \n" +
                      "0 - Codificar \n" +
                      "1 - Decodificar \n" +
                      "2 - Sair \n");

        return Console.ReadLine();
    }
    
    private static string? CallEncodersMenu()
    {
        Console.Write("\nSelecione um codificador: \n" +
                      "0 - Golomb \n" +
                      "1 - Elias-Gamma \n" +
                      "2 - Fibonacci \n" +
                      "3 - Unary \n" +
                      "4 - Delta \n");

        return Console.ReadLine();
    }
    
    private static string? CallPathMenu()
    {
        Console.Write("\nInforme o caminho do arquivo: \n");

        return Console.ReadLine();
    }
    
    private static string? CallOptionsPathMenu()
    {
        Console.Write("\nSelecione uma opção: \n" +
                      "0 - Caminho default (ultima codificação feita) \n" +
                      "1 - Digitar um caminho diferente \n");

        return Console.ReadLine();
    }
    
    private static string? CallDiverMenu()
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