using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RedLens.Tools.Exceptions;

namespace RedLens.Configuration;

/// <summary>
/// Contains extension methods for configuring Redis-related services.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly string sectionName = "RedLens";

    /// <summary>
    /// Adds RedLens services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configAction">An action to configure Redis settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configAction"/> is null.</exception>
    public static IServiceCollection AddRedLens(
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
                $"\"RedLens\" section must be provided with property \"ConnectionString\" with a valid redis database connection string.");
        }

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.ConnectionString));

        var config = new RedisConfiguration(services);
        configAction?.Invoke(config);
        return services;
    }
}
