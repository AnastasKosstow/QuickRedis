using Redis.Configuration;
using Redis.Stream;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(config =>
    {
        config.AddRedisStreaming();
    });

var app = builder.Build();

app.MapGet("/publish", async (IStreamPublisher streamPublisher) =>
{
    var request = new Request() { Content = "Value" };

    // For Subscribe project
    // in TBS.Example.Subscribe test bothasync and sync subscriptions
    // these names are for easy recognition of the channels
    await streamPublisher.PublishAsync("async", request);
    await streamPublisher.PublishAsync("sync", request);
});

app.Run();

public class Request
{
    public string Content { get; set; }
}
