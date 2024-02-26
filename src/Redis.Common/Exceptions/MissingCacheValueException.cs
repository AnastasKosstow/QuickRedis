namespace RedLens.Common.Exceptions;

public class MissingCacheValueException : Exception
{
    public MissingCacheValueException()
    {
    }

    public MissingCacheValueException(string message, params object[] args)
        : base(string.Format(message, args))
    {
    }
}
