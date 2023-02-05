using Microsoft.Extensions.DependencyInjection;
using Redis.Stream;
using Redis.Cache;
using Redis.Common.Serialization;

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
        services.AddSingleton<ICache, RedisCache>();
        return this;
    }

    public IRedisConfiguration AddRedisStreaming()
    {
        services
            .AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
            .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
        return this;
    }
}
