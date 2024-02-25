using QuickRedis.Cache;
using QuickRedis.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddQuickRedis(config =>
    {
        config.AddRedisCache();
    });

var app = builder.Build();

app.MapGet("/cache", async (IRedisCache cache) =>
{
    var key = "key";
    var value = "value";

    // Set
    await cache.SetAsync(key, value);
    
    // Set with expiration
    await cache.SetAsync(key, value, cacheEntryOptions =>
    {
        cacheEntryOptions.Expiration = new TimeSpan(1, 0, 0);
    });

    // Get
    var result = await cache.GetAsync(key);
});

app.Run();
