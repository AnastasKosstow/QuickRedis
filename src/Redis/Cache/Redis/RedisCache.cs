using System.Text.Json;
using StackExchange.Redis;
using Redis.Cache.Options;

namespace Redis.Cache;

internal sealed class RedisCache : ICache, IDisposable
{
    private IDatabase redisCache;

    private volatile IConnectionMultiplexer connection;

    private readonly SemaphoreSlim connectionLock = new(initialCount: 1, maxCount: 1);

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
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
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
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        cancellationToken.ThrowIfCancellationRequested();
        await ConnectAsync(cancellationToken);

        var result = await redisCache.StringGetAsync(key);
        if (!result.HasValue)
        {
            throw new ArgumentNullException(nameof(result));
        }
        return result;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
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
            if (redisCache == null)
            {
                redisCache = connection.GetDatabase();
            }
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
        }
    }
}
