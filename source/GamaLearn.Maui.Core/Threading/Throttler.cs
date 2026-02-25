namespace GamaLearn.Threading;

/// <summary>
/// Throttles calls to an action, ensuring it executes at most once per time interval.
/// Unlike debouncing, throttling executes immediately and then prevents further calls until the interval passes.
/// Useful for scroll handlers, mouse move events, and API rate limiting.
/// </summary>
public sealed partial class Throttler : IDisposable
{
    #region Fields
    private readonly TimeSpan interval;
    private readonly Lock tcsLock = new();
    private DateTime lastExecutionTime = DateTime.MinValue;
    private Action? pendingAction;
    private CancellationTokenSource? pendingCts;
    private bool disposed;
    #endregion

    /// <summary>
    /// Creates a new throttler with the specified interval.
    /// </summary>
    /// <param name="interval">The minimum interval between executions.</param>
    public Throttler(TimeSpan interval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);
        this.interval = interval;
    }

    /// <summary>
    /// Throttles the action. Executes immediately if the interval has passed,
    /// otherwise schedules execution for when the interval completes.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="executeTrailing">If true, executes the last call after the interval. If false, drops trailing calls.</param>
    public void Throttle(Action action, bool executeTrailing = true)
    {
        ArgumentNullException.ThrowIfNull(action);
        ObjectDisposedException.ThrowIf(disposed, this);

        lock (tcsLock)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan elapsed = now - lastExecutionTime;

            if (elapsed >= interval)
            {
                // Interval has passed, execute immediately
                lastExecutionTime = now;
                pendingAction = null;
                pendingCts?.Cancel();
                pendingCts?.Dispose();
                pendingCts = null;

                action();
            }
            else if (executeTrailing)
            {
                // Schedule for later
                pendingAction = action;
                pendingCts?.Cancel();
                pendingCts?.Dispose();
                pendingCts = new CancellationTokenSource();

                TimeSpan delay = interval - elapsed;
                CancellationToken token = pendingCts.Token;

                _ = Task.Delay(delay, token).ContinueWith(_ =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    Action? actionToExecute;
                    lock (tcsLock)
                    {
                        actionToExecute = pendingAction;
                        pendingAction = null;
                        lastExecutionTime = DateTime.UtcNow;
                    }

                    actionToExecute?.Invoke();
                }, token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
            }
            // If not executeTrailing, we simply drop this call
        }
    }

    /// <summary>
    /// Throttles the async action. Executes immediately if the interval has passed.
    /// </summary>
    /// <param name="action">The async action to execute.</param>
    /// <param name="executeTrailing">If true, executes the last call after the interval.</param>
    /// <returns>A task that completes when the action executes or is throttled.</returns>
    public async Task ThrottleAsync(Func<Task> action, bool executeTrailing = true)
    {
        ArgumentNullException.ThrowIfNull(action);
        ObjectDisposedException.ThrowIf(disposed, this);

        bool shouldExecuteNow;
        TimeSpan delay;

        lock (tcsLock)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan elapsed = now - lastExecutionTime;

            if (elapsed >= interval)
            {
                lastExecutionTime = now;
                shouldExecuteNow = true;
                delay = TimeSpan.Zero;
            }
            else
            {
                shouldExecuteNow = false;
                delay = interval - elapsed;
            }
        }

        if (shouldExecuteNow)
        {
            await action().ConfigureAwait(false);
        }
        else if (executeTrailing)
        {
            await Task.Delay(delay).ConfigureAwait(false);

            lock (tcsLock)
            {
                lastExecutionTime = DateTime.UtcNow;
            }

            await action().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Resets the throttler, allowing immediate execution on the next call.
    /// </summary>
    public void Reset()
    {
        lock (tcsLock)
        {
            lastExecutionTime = DateTime.MinValue;
            pendingAction = null;
            pendingCts?.Cancel();
            pendingCts?.Dispose();
            pendingCts = null;
        }
    }

    /// <summary>
    /// Cancels any pending throttled action.
    /// </summary>
    public void Cancel()
    {
        lock (tcsLock)
        {
            pendingAction = null;
            pendingCts?.Cancel();
            pendingCts?.Dispose();
            pendingCts = null;
        }
    }

    /// <summary>
    /// Gets the time remaining until the next execution is allowed.
    /// Returns <see cref="TimeSpan.Zero"/> if execution is allowed immediately.
    /// </summary>
    public TimeSpan TimeUntilNextAllowed
    {
        get
        {
            lock (tcsLock)
            {
                TimeSpan elapsed = DateTime.UtcNow - lastExecutionTime;
                TimeSpan remaining = interval - elapsed;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }
        }
    }

    /// <summary>
    /// Gets whether there is a pending throttled action.
    /// </summary>
    public bool HasPending
    {
        get
        {
            lock (tcsLock)
            {
                return pendingAction is not null;
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;

        lock (tcsLock)
        {
            pendingAction = null;
            pendingCts?.Cancel();
            pendingCts?.Dispose();
            pendingCts = null;
        }
    }
}