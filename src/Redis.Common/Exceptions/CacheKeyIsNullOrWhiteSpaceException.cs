namespace Redis.Common.Exceptions;

public class CacheKeyIsNullOrWhiteSpaceException : Exception
{
    public CacheKeyIsNullOrWhiteSpaceException()
    {
    }

    public CacheKeyIsNullOrWhiteSpaceException(string message, params object[] args)
        : base(string.Format(message, args))
    {
    }
}
