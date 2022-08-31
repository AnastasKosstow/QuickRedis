namespace Redis.Stream;

public interface IStreamSubscriber
{
    Task SubscribeAsync<T>(string channel, Action<T> handler) 
        where T : class;

    void Subscribe<T>(string channel, Action<T> handler)
        where T : class;
}
