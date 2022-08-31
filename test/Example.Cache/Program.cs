using Redis.Cache;
using Redis.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(config =>
    {
        config.AddRedisCache();
    });

var app = builder.Build();

app.MapGet("/cache", async (ICache cache) =>
{
    var key = "key";
    var value = "value";

    await cache.SetAsync(key, value);
    await cache.SetAsync(key, value, x => x.Expiration = new TimeSpan(1, 0, 0));
    var result = await cache.GetAsync(key);
});

app.Run();
