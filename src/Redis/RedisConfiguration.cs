using Microsoft.Extensions.DependencyInjection;
using RedLens.Stream;
using RedLens.Cache;
using RedLens.Tools.Serialization;
using RedLens.Stream.Abstractions;

namespace RedLens.Configuration;

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
