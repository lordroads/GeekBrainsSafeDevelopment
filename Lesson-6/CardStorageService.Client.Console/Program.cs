using CardStorageServiceProtos;
using Grpc.Net.Client;
using System;
using static CardStorageServiceProtos.CardService;
using static CardStorageServiceProtos.CliectService;

namespace CardStorageService.Client.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //The System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport switch is only required for .NET Core 3.x. It does nothing in .NET 5 and isn't required.
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var httpHandler = new HttpClientHandler();

            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var chennal = GrpcChannel.ForAddress("https://localhost:5001",
                new GrpcChannelOptions { HttpHandler = httpHandler });

            CliectServiceClient clientService = new CliectServiceClient(chennal);

            var createClientResponse = clientService.Create(new CreateClientRequest { 
                FirstName = "Grpc",
                Surname = "Grpc_seurname",
                Patronymic = "Grpc_patronymic"
            });

            Console.WriteLine($"Client {createClientResponse.ClientId} created." +
                $"\nError code: {createClientResponse.ErrorCode}" +
                $"\nError message: {createClientResponse.ErrorMessage}");

            CardServiceClient cardService = new CardServiceClient(chennal);

            var getByClientIdResponse = cardService.GetById(new GetCardsRequest
            {
                ClientId = "1"
            });

            Console.WriteLine("Cards:\n**************");
            foreach (var item in getByClientIdResponse.Cards)
            {
                Console.WriteLine($"{item.CardNo}; {item.Name}; {item.CVV2}; {item.ExpDate};");
            }

            Console.ReadKey();
        }
    }
}