using Grpc.Core;
using RpcDemo;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {

        var channel = new Channel("localhost:50051", ChannelCredentials.Insecure);

        var eventClient = new EventService.EventServiceClient(channel);
        var serviceClient = new TempService.TempServiceClient(channel);

        var subscription = new EventSubscription();

        subscription.EventTypes.Add("ALARM");
        subscription.EventTypes.Add("STATUS");

        using var call = eventClient.Subscribe(subscription);

        _ = Task.Run(async () =>
        {
            while (await call.ResponseStream.MoveNext())
            {
                var evt = call.ResponseStream.Current;

                Console.WriteLine($"EVENT: {evt.Type} - {evt.Text}");
            }
        });

        while (true)
        {
            var cmd = Console.ReadLine();

            var parts = cmd.Split(' ');

            var command = (RpcDemo.CommandType)int.Parse(parts[0]);

            var msg = new ClientMessage();

            msg.TempRequest = new TemperatureRequest() { Command = command };

            switch (command)
            {
                case CommandType.ReadValue:
                    break;
                case CommandType.SetValue:
                case CommandType.IncreaseValue:
                case CommandType.DecreaseValue:
                    if (parts.Length > 1) msg.TempRequest.Value = double.Parse(parts[1]);
                    else msg.TempRequest.Value = 10.0;
                        break;
                default:
                    break;
            }

            var reply = await serviceClient.ExecuteAsync(msg);

            Console.WriteLine(reply);
        }
    }
}

