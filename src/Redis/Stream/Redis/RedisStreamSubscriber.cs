using StackExchange.Redis;
using Redis.Common.Extensions;
using Redis.Common.Serialization;

namespace Redis.Stream;

public sealed class RedisStreamSubscriber : IStreamSubscriber
{
    private readonly ISubscriber subscriber;
    private readonly ISerializer serializer;

    public RedisStreamSubscriber(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
    {
        this.subscriber = connectionMultiplexer.GetSubscriber();
        this.serializer = serializer;
    }

    public Task SubscribeAsync<T>(string channel, Action<T> handler) 
        where T : class
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(channel));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var result = subscriber.SubscribeAsync(channel, (_, data) =>
        {
            var payload = serializer.Deserialize<T>(data);
            if (payload is null)
            {
                return;
            }

            handler(payload);
        });

        return result;
    }

    public void Subscribe<T>(string channel, Action<T> handler)
        where T : class
    {
        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(channel));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var subscription = subscriber
            .SubscribeAsync(channel)
            .WaitAndUnwrapException();

        subscription.OnMessage(message =>
        {
            var payload = serializer.Deserialize<T>(message.Message);
            handler(payload);
        });
    }
}
