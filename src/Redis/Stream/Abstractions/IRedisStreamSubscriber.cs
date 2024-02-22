namespace Redis.Stream.Abstractions;

public interface IRedisStreamSubscriber
{
    Task SubscribeAsync<T>(string channel, Action<T> handler) where T : class;

    void Subscribe<T>(string channel, Action<T> handler) where T : class;
}
