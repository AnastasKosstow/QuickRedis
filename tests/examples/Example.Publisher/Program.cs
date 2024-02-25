using QuickRedis.Configuration;
using QuickRedis.Stream.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddQuickRedis(config =>
    {
        config.AddRedisStreaming();
    });

var app = builder.Build();

app.MapGet("/publish", async (IRedisStreamPublisher streamPublisher) =>
{
    var request = new Request() { Content = "Value" };

    // For Subscribe project
    // in TBS.Example.Subscribe test bothasync and sync subscriptions
    // these names are for easy recognition of the channels
    await streamPublisher.PublishAsync("async", request);
    await streamPublisher.PublishAsync("sync", request);
});

app.MapPost("/publish", async (IRedisStreamPublisher streamPublisher) =>
{
    var request = new Request() 
    { 
        Content = "Hello, Redis Stream!"
    };

    await streamPublisher.PublishAsync("request_queue", request);
});

app.Run();

public class Request
{
    public string Content { get; set; }
}
