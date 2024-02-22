using Redis.Cache.Options;

namespace Redis.Cache;

public interface IRedisCache
{
    Task SetAsync(string key, string value, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, Action<CacheEntryOptions> options, CancellationToken cancellationToken = default);

    Task<string> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<(bool Success,string Result)> TryGetAsync(string key, CancellationToken cancellationToken = default);
    Task<(bool Success, T Result)> TryGetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
