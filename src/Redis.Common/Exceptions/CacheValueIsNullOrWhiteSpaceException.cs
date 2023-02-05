namespace Redis.Common.Exceptions;

public class CacheValueIsNullOrWhiteSpaceException : Exception
{
    public CacheValueIsNullOrWhiteSpaceException()
    {
    }

    public CacheValueIsNullOrWhiteSpaceException(string message, params object[] args)
        : base(string.Format(message, args))
    {
    }
}
