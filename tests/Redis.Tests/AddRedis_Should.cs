using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLens.Configuration;
using RedLens.Tools.Exceptions;
using StackExchange.Redis;

namespace RedLens.Tests;

public class AddRedis_Should
{
    [Test]
    public void AddRedisToServiceCollection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[] { new KeyValuePair<string, string>("RedLens:ConnectionString", "localhost:6379") })
            .Build();

        services
            .AddSingleton<IConfiguration>(configuration)
            .AddRedLens(config =>
            {
            });

        var serviceProvider = services.BuildServiceProvider();
        var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();

        Assert.That(connectionMultiplexer, Is.Not.Null);
    }

    [Test]
    public void ThrowRedisConfigurationOptionsExceptionIfMissingConnectionString()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[] { new KeyValuePair<string, string>("redis:", "") })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        try
        {
            services.AddRedLens(config =>
            {
            });
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(RedisConfigurationOptionsException)));
        }
    }

    [Test]
    public void ThrowRedisConfigurationOptionsExceptionIfMissingConfiguration()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        try
        {
            services.AddRedLens(config =>
            {
            });
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(RedisConfigurationOptionsException)));
        }
    }

    [Test]
    public void ThrowArgumentNullExceptionIfNullConfigAction()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        try
        {
            services.AddRedLens(null);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(ArgumentNullException)));
        }
    }
}
