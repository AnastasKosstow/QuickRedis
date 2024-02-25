Quick Start
==========================

## Configuration
To use the library in your project, follow these steps:
>
 - Add "QuickRedis" section in your appsetting.json file, and "ConnectionString" with valid redis connection.
   ```json
     "QuickRedis": {
       "ConnectionString": "localhost:6379"
     }
   ```

 - Add QuickRedis Fluent library in Program.cs with the functionality you want:
   - Cache:
      ```C#
      builder.Services.AddQuickRedis(config => config.AddRedisCache());
      ```
 
    - Streaming
      ```C#
      builder.Services.AddQuickRedis(config => config.AddRedisStreaming());
      ```

    - Or both
      ```C#
      builder.Services
         .AddQuickRedis(config =>
         {
             config
                 .AddRedisCache() // Add redis cache
                 .AddRedisStreaming(); // Add redis Pub/Sub
         });
      ```

# Cache
To use Redis cache, inject "ICache" interface from "QuickRedis.Cache" namespace

### Features
 - Provides a simple and intuitive way to interact with Redis cache.
 - Supports Redis operations like Get, Set, and Remove.
 - Supports serialization and deserialization of complex objects.
 - Thread-safe access to Redis database.
 - Provides full control over cache entry options, such as expiration time.
 - Safe and robust handling of connection errors and exceptions.

### Cache Operations:

Assuming we injected an ICache interface
```C#
  private readonly ICache cache;

  public MyConstructor(ICache cache)
  {
      this.cache = cache;
  }
```

---

 - 𝚂𝚎𝚝
   - 𝚂𝚎𝚝𝙰𝚜𝚢𝚗𝚌
   - 𝚂𝚎𝚝𝙰𝚜𝚢𝚗𝚌<𝚃>

  The `𝚂𝚎𝚝𝙰𝚜𝚢𝚗𝚌` method put data to the cache. 
   - If the key does not exists, throws `CacheKeyIsNullOrWhiteSpaceException`.
   - If the value does not exists, throws `CacheValueIsNullOrWhiteSpaceException`.

   ```C#
     // Set
     await cache.SetAsync(key, value);

     // Set with expiration
     await cache.SetAsync(key, value, cacheEntryOptions =>
     {
         cacheEntryOptions.Expiration = new TimeSpan(1, 0, 0);
     });
   ```

---

 - 𝙶𝚎𝚝
   - 𝙶𝚎𝚝𝙰𝚜𝚢𝚗𝚌
   - 𝙶𝚎𝚝𝙰𝚜𝚢𝚗𝚌<𝚃>
   - 𝚃𝚛𝚢𝙶𝚎𝚝𝙰𝚜𝚢𝚗𝚌
   - 𝚃𝚛𝚢𝙶𝚎𝚝𝙰𝚜𝚢𝚗𝚌<𝚃>
   
 The `GetAsync` method retrieves data from the cache. If the key exists, the method returns the value associated with the key; otherwise, it throws a `MissingCacheValueException`.

   ```C#
     await cache.GetAsync(key);
   ```

   ```C#
     var result = await cache.𝚃𝚛𝚢𝙶𝚎𝚝𝙰𝚜𝚢𝚗𝚌(key);
     if (result.Success)
     {
         var value = result.Result;
     }
   ```

---

 - 𝚁𝚎𝚖𝚘𝚟𝚎
   - 𝚁𝚎𝚖𝚘𝚟𝚎𝙰𝚜𝚢𝚗𝚌
   
 The `RemoveAsync` method allows you to delete data from the cache by key. If the key is null or empty, it throws a `CacheKeyIsNullOrWhiteSpaceException`. 

   ```C#
     await cache.RemoveAsync(key);
   ```

---

# Stream

QuickRedis library supports Publish/Subscribe (Pub/Sub) messaging patterns through Redis streams, enabling applications to communicate asynchronously via messages. <br>
This section describes how to publish messages to a stream and subscribe to receive messages from a stream.

 - **Publish**
   
   To publish messages to a Redis stream, inject the `IRedisStreamPublisher` into your controllers or services. Use the `PublishAsync` method to send messages to the specified stream. <br>

   PublishAsync method takes two parameters:
    - `queue`: *string*, the name of the queue to which the data is sent
    - `data`: *T*, the object that is send to the queue
      
   ```C#
   var request = new Request() 
   { 
       Content = "Hello, Redis Stream!"
   };
   await streamPublisher.PublishAsync("request_queue", request);

   var str = "some_string";
   await streamPublisher.PublishAsync("string_queue", str);
   ```

 - **Subscribe**

   Use the `IRedisStreamSubscriber` interface to subscribe to channels either asynchronously or synchronously. <br>
   IRedisStreamSubscriber contains two method:
    - 𝚂𝚞𝚋𝚜𝚌𝚛𝚒𝚋𝚎𝙰𝚜𝚢𝚗𝚌<𝚃>
    - 𝚂𝚞𝚋𝚜𝚌𝚛𝚒𝚋𝚎<𝚃>
   
   both methods require two parameters:
    - `channel`: string, the channel on which it will listen for messages
    - `handler`: 𝖠𝖼𝗍𝗂𝗈𝗇<𝖳>, how to handle the incoming messages

   ```C#
   internal sealed class AsyncSubscriberBackgroundService : BackgroundService
   {
       private readonly IRedisStreamSubscriber streamSubscriber;

       public AsyncSubscriberBackgroundService(IRedisStreamSubscriber streamSubscriber)
       {
           this.streamSubscriber = streamSubscriber;
       }

       protected override async Task ExecuteAsync(CancellationToken cancellationToken)
       {
           await streamSubscriber.SubscribeAsync<Response>("request_queue", message =>
           {
            Console.WriteLine(message.Content);
           });
       }
   }
   ```







