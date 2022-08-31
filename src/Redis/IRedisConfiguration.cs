namespace Redis.Configuration;

public interface IRedisConfiguration
{
    IRedisConfiguration AddRedisStreaming();
    IRedisConfiguration AddRedisCache();
}
