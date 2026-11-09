using Grpc.Core;
using RpcDemo;
using System.Net.NetworkInformation;
using System.Threading.Tasks;



public class TempServiceImpl : TempService.TempServiceBase
{
    private int _status = 0;
    private double _setpoint = 0.0;


    public double SetPoint
    {
        get { return _setpoint; }
    }

    //public override async Task Connect(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
    //{
    //    var queue = Channel.CreateUnbounded<ServerMessage>();

    //    //
    //    // Event generator thread
    //    //
    //    _ = Task.Run(async () =>
    //    {
    //        while (!context.CancellationToken.IsCancellationRequested)
    //        {
    //            await Task.Delay(5000);

    //            await queue.Writer.WriteAsync(
    //                new ServerMessage
    //                {
    //                    Alarm =
    //                        new AlarmEvent
    //                        {
    //                            Text = "Random Alarm"
    //                        }
    //                });
    //        }
    //    });

    //    //
    //    // Sender thread
    //    //
    //    var senderTask = Task.Run(async () =>
    //    {
    //        await foreach (
    //            var msg in queue.Reader.ReadAllAsync())
    //        {
    //            await responseStream.WriteAsync(msg);
    //        }
    //    });

    //    //
    //    // Incoming commands
    //    //
    //    await foreach (
    //        var request in requestStream.ReadAllAsync())
    //    {
    //        switch (request.PayloadCase)
    //        {
    //            case ClientMessage.PayloadOneofCase.Ping:

    //                await queue.Writer.WriteAsync(
    //                    new ServerMessage
    //                    {
    //                        Pong = new Pong()
    //                    });

    //                break;

    //            case ClientMessage.PayloadOneofCase.ReadTemperature:

    //                await queue.Writer.WriteAsync(
    //                    new ServerMessage
    //                    {
    //                        Temperature =
    //                            new TemperatureResponse
    //                            {
    //                                Value = 23.7
    //                            }
    //                    });

    //                break;
    //        }
    //    }

    //    queue.Writer.Complete();

    //    await senderTask;
    //}

    public override Task<ServerMessage> Execute(ClientMessage clientmsg, ServerCallContext context)
    { 
        var msg = "";
       
        if (clientmsg.PayloadCase == ClientMessage.PayloadOneofCase.TempRequest)
        {

            var request = clientmsg.TempRequest;



            Console.WriteLine($"Received from client: {request.Command}, Value: {request.Value}, Data Size: {request.Data.Count}");

            switch (request.Command)
            {
                case CommandType.ReadValue:
                    msg = $"Current value: {_setpoint}";
                    break;

                case CommandType.SetValue:
                    _setpoint = request.Value;
                    msg = $"Setpoint updated to: {_setpoint}";
                    break;

                case CommandType.IncreaseValue:
                    _setpoint += request.Value;
                    msg = $"Setpoint increased to: {_setpoint}";
                    break;

                case CommandType.DecreaseValue:
                    _setpoint -= request.Value;
                    msg = $"Setpoint decreased to: {_setpoint}";
                    break;

                default:
                    msg = $"Unknown command: {request.Command}";
                    break;

            }
        }
        else
        {
            msg = "Invalid Payload";
        }

        var tempResp = new TemperatureResponse() { Message = msg, Status = _status, Value = _setpoint };

        var response = new ServerMessage() { TempResponse = tempResp };

        return Task.FromResult(response);

    }
}
