using System.Text.Json;
using StackExchange.Redis;
using Redis.Cache.Options;
using Redis.Common.Exceptions;

namespace Redis.Cache;

internal sealed class RedisCache : ICache, IDisposable
{
    private IDatabase redisCache;

    private readonly SemaphoreSlim connectionLock = new(initialCount: 1, maxCount: 1);

    /// <summary>
    /// Use "volatile" instead of "readonly". 
    /// This is because the connection object is assigned in the constructor and can also be changed later in the ConnectAsync method.
    /// Using volatile in this case ensures that the most up-to-date value of connection is accessed across multiple threads and its state can be changed by any thread at any time,
    /// without causing unexpected behavior.
    /// </summary>
    private volatile IConnectionMultiplexer connection;
    
    public RedisCache(IConnectionMultiplexer connection)
    {
        this.connection = connection;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var redisValue = JsonSerializer.Serialize(value);
        await SetAsync(key, redisValue, null, cancellationToken);
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await SetAsync(key, value, null, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default)
    {
        var redisValue = JsonSerializer.Serialize(value);
        await SetAsync(key, redisValue, options, cancellationToken);
    }

    public async Task SetAsync(string key, string value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new CacheKeyIsNullOrWhiteSpaceException(nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new CacheValueIsNullOrWhiteSpaceException(nameof(value));
        }

        cancellationToken.ThrowIfCancellationRequested();
        await ConnectAsync(cancellationToken);

        if (options != null)
        {
            var entryOptions = new CacheEntryOptions();
            options?.Invoke(entryOptions);

            await redisCache.StringSetAsync(key, value, entryOptions.Expiration);
        }
        else
        {
            await redisCache.StringSetAsync(key, value);
        }
    }

    public async Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new CacheKeyIsNullOrWhiteSpaceException(nameof(key));
        }

        cancellationToken.ThrowIfCancellationRequested();
        await ConnectAsync(cancellationToken);

        var result = await redisCache.StringGetAsync(key);
        if (!result.HasValue)
        {
            throw new MissingCacheValueException($"Redis cache does not contains value for key: {key}");
        }
        return result;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new CacheKeyIsNullOrWhiteSpaceException(nameof(key));
        }

        await ConnectAsync(cancellationToken);
        await redisCache.KeyDeleteAsync(key);
    }

    private async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (redisCache != null)
        {
            return;
        }

        await connectionLock.WaitAsync(cancellationToken);
        try
        {
            redisCache ??= connection.GetDatabase();
        }
        finally
        {
            connectionLock.Release();
        }
    }

    public void Dispose()
    {
        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
        }
        connectionLock.Dispose();
    }
}
