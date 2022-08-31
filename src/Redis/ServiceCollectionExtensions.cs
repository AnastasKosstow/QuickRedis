using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis.Configuration;

public static class ServiceCollectionExtensions
{
    private static readonly string sectionName = "redis";

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        Action<IRedisConfiguration> configAction)
    {
        if (configAction is null)
        {
            throw new ArgumentNullException($"{nameof(configAction)} is null. Configuration cannot be null or empty!");
        }

        IConfiguration configuration;
        using (var serviceProvider = services.BuildServiceProvider())
        {
            configuration = serviceProvider.GetService<IConfiguration>();
        }

        var section = configuration.GetRequiredSection(sectionName);
        var options = new RedisOptions();
        section.Bind(options);

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString));

        var config = new RedisConfiguration(services);
        configAction?.Invoke(config);
        return services;
    }
}
