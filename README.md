Documentation
==========================

> A .NET library for easy integration of Redis in a fluent manner.
> To use the library in your project, follow these steps:

Add "redis" section in your appsetting.json file, and "ConnectionString" with valid redis connection.

```json
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
```


* [Cache](#cache)

Cache
--------------

Features
 - Provides a simple and intuitive way to interact with Redis cache.
 - Supports Redis string operations like Get, Set, and Remove.
 - Supports serialization and deserialization of complex objects using System.Text.Json.
 - Thread-safe access to Redis database using a SemaphoreSlim lock.
 - Provides full control over cache entry options, such as expiration time.
 - Safe and robust handling of connection errors and exceptions.




