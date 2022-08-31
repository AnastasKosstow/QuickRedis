namespace Redis.Stream;

public interface IStreamPublisher
{
    Task PublishAsync<T>(string queue, T data)
        where T : class;
}