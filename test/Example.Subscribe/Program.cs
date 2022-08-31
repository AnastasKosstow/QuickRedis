using Redis.Configuration;
using Redis.Stream;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(config =>
    {
        config.AddRedisStreaming();
    })
    .AddHostedService<SyncSubscriberBackgroundService>()
    .AddHostedService<AsyncSubscriberBackgroundService>();

var app = builder.Build();
app.Run();

internal sealed class AsyncSubscriberBackgroundService : BackgroundService
{
    private readonly IStreamSubscriber streamSubscriber;

    public AsyncSubscriberBackgroundService(IStreamSubscriber streamSubscriber)
    {
        this.streamSubscriber = streamSubscriber;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await streamSubscriber.SubscribeAsync<Response>("async", message =>
        {
            Console.WriteLine(message.Content);
        });
    }
}

internal sealed class SyncSubscriberBackgroundService : BackgroundService
{
    private readonly IStreamSubscriber streamSubscriber;

    public SyncSubscriberBackgroundService(IStreamSubscriber streamSubscriber)
    {
        this.streamSubscriber = streamSubscriber;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        streamSubscriber.Subscribe<Response>("sync", message =>
        {
            Console.WriteLine(message.Content);
        });

        return Task.CompletedTask;
    }
}

class Response
{
    public string Content { get; set; }
}
