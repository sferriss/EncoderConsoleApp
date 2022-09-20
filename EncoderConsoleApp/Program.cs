// See https://aka.ms/new-console-template for more information

using EncoderConsoleApp.Factories;
using EncoderConsoleApp.Handlers.Encoders;
using EncoderConsoleApp.Handlers.Encoders.Interfaces;
using EncoderConsoleApp.Handlers.Files;
using EncoderConsoleApp.Run;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IEncoderFactory, EncoderFactory>();
services.AddTransient<IFileHandler, FileHandler>();
services.AddTransient<IUnaryEncoder, UnaryEncoder>();
services.AddTransient<IDeltaEncoder, DeltaEncoder>();
services.AddTransient<IEliasGammaEncoder, EliasGammaEncoder>();
services.AddTransient<IFibonacciEncoder, FibonacciEncoder>();
services.AddTransient<IGolombEncoder, GolombEncoder>();

services.AddTransient<RunProject>();

services.BuildServiceProvider().GetService<RunProject>()!.Run();