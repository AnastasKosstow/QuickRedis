using StackExchange.Redis;
using Redis.Common.Serialization;

namespace Redis.Stream;

public sealed class RedisStreamPublisher : IStreamPublisher
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
        return subscriber.PublishAsync(queue, payload);
    }
}
