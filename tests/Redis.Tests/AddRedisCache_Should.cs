using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLens.Cache;
using RedLens.Configuration;
using RedLens.Tools.Exceptions;

namespace RedLens.Tests;

public class AddRedisCache_Should
{
    [Test]
    public async Task SetAsyncWithValidKeyAndValue()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "key";
        string value = "value";

        try
        {
            await redisCache.SetAsync(key, value);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task ThrowsExceptionIfKeyIsNullOrEmpty_SetAsync()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "";
        string value = "value";

        try
        {
            await redisCache.SetAsync(key, value);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task ThrowsExceptionIfValueIsNullOrEmpty_SetAsync()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "key";
        string value = "";

        try
        {
            await redisCache.SetAsync(key, value);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheValueIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task GetAsyncWithValidKey()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "key";
        string value = "value";
        
        try
        {
            await redisCache.SetAsync(key, value);
            await redisCache.GetAsync(key);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task ThrowsExceptionIfKeyIsNullOrEmpty_GetAsync()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "";

        try
        {
            await redisCache.GetAsync(key);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task RemoveAsyncWithValidKey()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "key";
        string value = "value";

        try
        {
            await redisCache.SetAsync(key, value);
            await redisCache.RemoveAsync(key);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }

    [Test]
    public async Task ThrowsExceptionIfKeyIsNullOrEmpty_RemoveAsync()
    {
        var serviceProvider = services.BuildServiceProvider();
        var redisCache = serviceProvider.GetService<IRedisCache>();

        string key = "";

        try
        {
            await redisCache.GetAsync(key);
        }
        catch (Exception exception)
        {
            Assert.That(exception.GetType(), Is.EqualTo(typeof(CacheKeyIsNullOrWhiteSpaceException)));
        }
    }


    #region ARRANGE

    private readonly IServiceCollection services;
    private readonly IConfiguration configuration;

    public AddRedisCache_Should()
    {
        services = new ServiceCollection();
        configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[] { new KeyValuePair<string, string>("redis:ConnectionString", "localhost:6379") })
            .Build();

        services
            .AddSingleton(configuration)
            .AddRedLens(config =>
            {
                config.AddRedisCache();
            });
    }

    #endregion
}
