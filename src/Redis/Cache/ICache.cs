using Redis.Cache.Options;

namespace Redis.Cache;

public interface ICache
{
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default);

    Task<string> GetAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
