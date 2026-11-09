using Grpc.Core;
using RpcDemo;

public class EventServiceImpl : EventService.EventServiceBase
{
    private TempServiceImpl _tempService;
    private double _limit;

    public override async Task Subscribe(EventSubscription request, IServerStreamWriter<EventMessage> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000);

            var currentTemp = _tempService.SetPoint;

            Console.WriteLine($"Temperature: {currentTemp}");

            if (currentTemp > _limit)
            {
                await responseStream.WriteAsync(new EventMessage
                    {
                        Type = EventType.Alarm,
                        Text = $"Over-temperature alarm: {currentTemp}",
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    });
            }
        }
    }

    public EventServiceImpl(TempServiceImpl tempService, double limit)
    {
        _tempService = tempService;
        _limit = limit;
    }
}
