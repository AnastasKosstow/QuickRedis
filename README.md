Documentation
==========================

To use the library in your project, follow these steps:
>
 - Add "Redis" section in your appsetting.json file, and "ConnectionString" with valid redis connection.

```json
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
```

 - Add Redis Fluent library in Program.cs with the functionality you want:
```C#
  builder.Services
    .AddRedis(config =>
    {
        config
            .AddRedisCache() // Add redis cache
            .AddRedisStreaming(); // Add redis Pub/Sub
    });
```


# Cache
To use Redis cache, inject "ICache" interface from "Redis.Cache" namespace

### Features
 - Provides a simple and intuitive way to interact with Redis cache.
 - Supports Redis operations like Get, Set, and Remove.
 - Supports serialization and deserialization of complex objects using System.Text.Json.
 - Thread-safe access to Redis database using a SemaphoreSlim lock.
 - Provides full control over cache entry options, such as expiration time.
 - Safe and robust handling of connection errors and exceptions.

### Supported operations:

<i>SetAsync, SetAsync<T></i>
```C#
  // Set
  await cache.SetAsync(key, value);

  // Set with expiration
  await cache.SetAsync(key, value, cacheEntryOptions =>
  {
      cacheEntryOptions.Expiration = new TimeSpan(1, 0, 0);
  });
```

<i>GetAsync</i>
```C#
  await cache.GetAsync(key);
```

<i>RemoveAsync</i>
```C#
  await cache.RemoveAsync(key);
```

