namespace Redis.Stream.Abstractions;

public interface IRedisStreamPublisher
{
    Task PublishAsync<T>(string queue, T data) where T : class;
}