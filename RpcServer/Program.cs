using Grpc.Core;
using RpcDemo;

class Program
{
    static void Main(string[] args)
    {
        const int Port = 50051;

        var tempService = new TempServiceImpl();
        var eventService = new EventServiceImpl(tempService, 50.5);

        var server = new Server
        {
            Services = {
                TempService.BindService(tempService),
                EventService.BindService(eventService)
            },
            Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };

        server.Start();

        Console.WriteLine($"Server listening on port {Port}");
        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        server.ShutdownAsync().Wait();
    }
}

