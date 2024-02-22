using StackExchange.Redis;
using Redis.Common.Serialization;
using System.Runtime.Serialization;
using Redis.Stream.Abstractions;

namespace Redis.Stream;

public sealed class RedisStreamPublisher : IRedisStreamPublisher
{
    private readonly ISubscriber subscriber;
    private readonly ISerializer serializer;

    public RedisStreamPublisher(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
    {
        this.subscriber = connectionMultiplexer.GetSubscriber();
        this.serializer = serializer;
    }

    public Task PublishAsync<T>(string queue, T data)
        where T : class
    {
        var payload = serializer.Serialize(data);
        if (payload is null)
        {
            string message = $"In {nameof(PublishAsync)}\nCannot serialize payload: {data}";
            throw new SerializationException(message);
        }

        return subscriber.PublishAsync(queue, payload);
    }
}
