using Microsoft.Extensions.DependencyInjection;
using Redis.Stream;
using Redis.Cache;
using Redis.Common.Serialization;
using Redis.Stream.Abstractions;

namespace Redis.Configuration;

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
