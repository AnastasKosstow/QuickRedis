using System.Runtime.ExceptionServices;

namespace Redis.Common.Helpers;

public static class ExceptionHelpers
{
    /// <summary>
    /// Attempts to prepare the exception for re-throwing by preserving the stack trace. 
    /// The returned exception should be immediately thrown.
    /// </summary>
    public static Exception PrepareForRethrow(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();

        // The code cannot ever get here. We just return a value to work around a badly-designed API (ExceptionDispatchInfo.Throw):
        return exception;
    }
}
