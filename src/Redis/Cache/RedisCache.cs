using System.Text.Json;
using StackExchange.Redis;
using Redis.Cache.Options;
using Redis.Common.Exceptions;

namespace Redis.Cache;

internal sealed class RedisCache : IRedisCache, IDisposable
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

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await SetAsync(key, value, null, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        string serializedValue;
        try
        {
            serializedValue = JsonSerializer.Serialize(value);
        }
        catch (JsonException ex)
        {
            throw new CacheSerializationException("Failed to serialize the value.", ex);
        }

        await SetAsync(key, serializedValue, null, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default)
    {
        string serializedValue;
        try
        {
            serializedValue = JsonSerializer.Serialize(value);
        }
        catch (JsonException ex)
        {
            throw new CacheSerializationException("Failed to serialize the value.", ex);
        }

        await SetAsync(key, serializedValue, options, cancellationToken);
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

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var redisValue = await GetAsync(key, cancellationToken);
        try
        {
            T result = JsonSerializer.Deserialize<T>(redisValue);
            return result;
        }
        catch (JsonException ex)
        {
            throw new CacheSerializationException("Failed to deserialize the result.", ex);
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

        RedisValue result = await redisCache.StringGetAsync(key);
        if (!result.HasValue)
        {
            throw new MissingCacheValueException($"Redis cache does not contain a value for key: {key}");
        }

        return result;
    }

    public async Task<(bool Success, string Result)> TryGetAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await TryGetAsync(key);
        if (result.Success)
        {
            var value = result.Result;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new CacheKeyIsNullOrWhiteSpaceException(nameof(key));
        }

        cancellationToken.ThrowIfCancellationRequested();
        await ConnectAsync(cancellationToken);

        RedisValue redisValue = await redisCache.StringGetAsync(key);
        if (!redisValue.HasValue)
        {
            return (false, null);
        }

        return (true, redisValue);
    }

    public async Task<(bool Success, T Result)> TryGetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var (Success, Result) = await TryGetAsync(key, cancellationToken);
        if (Success)
        {
            try
            {
                T deserializeResult = JsonSerializer.Deserialize<T>(Result);
                return (true, deserializeResult);
            }
            catch (JsonException ex)
            {
                throw new CacheSerializationException("Failed to deserialize the result.", ex);
            }
        }
        else
        {
            return (false, default(T));
        }
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
