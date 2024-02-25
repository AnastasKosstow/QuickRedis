using Microsoft.Extensions.DependencyInjection;
using QuickRedis.Stream;
using QuickRedis.Cache;
using QuickRedis.Common.Serialization;
using QuickRedis.Stream.Abstractions;

namespace QuickRedis.Configuration;

public class RedisConfiguration : IRedisConfiguration
{
    private readonly IServiceCollection services;

    public RedisConfiguration(IServiceCollection services)
    {
        this.services = services;
    }

    public IRedisConfiguration AddRedisCache()
    {
        services.AddSingleton<IRedisCache, RedisCache>();
        return this;
    }

    public IRedisConfiguration AddRedisStreaming()
    {
        services
            .AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddSingleton<IRedisStreamPublisher, RedisStreamPublisher>()
            .AddSingleton<IRedisStreamSubscriber, RedisStreamSubscriber>();
        return this;
    }
}
