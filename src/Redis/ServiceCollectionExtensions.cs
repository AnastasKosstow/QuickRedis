using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Redis.Common.Exceptions;

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

        var section = configuration.GetSection(sectionName);
        var options = new RedisOptions();
        section.Bind(options);

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            throw new RedisConfigurationOptionsException(
                $"Cannot get RedisOptions section from {nameof(IConfiguration)}. " +
                $"\"Redis\" section must be provided with property \"ConnectionString\" with valid redis database connection string.");
        }

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString));

        var config = new RedisConfiguration(services);
        configAction?.Invoke(config);
        return services;
    }
}
