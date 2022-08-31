using Redis.Common.Helpers;

namespace Redis.Common.Extensions;

public static class TaskExtensions
{
    public static void WaitAndUnwrapException(this Task task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        task.GetAwaiter().GetResult();
    }

    public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        return task.GetAwaiter().GetResult();
    }

    public static void WaitAndUnwrapException(this Task task, CancellationToken cancellationToken)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        try
        {
            task.Wait(cancellationToken);
        }
        catch (AggregateException ex)
        {
            throw ExceptionHelpers.PrepareForRethrow(ex.InnerException);
        }
    }
}
