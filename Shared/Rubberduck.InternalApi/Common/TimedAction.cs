using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Common;

public static class TimedAction
{
    public static TimeSpan Run(Action action)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            action.Invoke();
        }
        finally
        {
            sw.Stop();
        }
        return sw.Elapsed;
    }

    public static bool TryRun(Action action, out TimeSpan elapsed, out Exception? exception)
    {
        exception = default;
        var sw = Stopwatch.StartNew();
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            sw.Stop();
        }

        elapsed = sw.Elapsed;
        return exception == default;
    }

    public async static Task<(bool Success, TimeSpan Elapsed, Exception? Exception)> TryRunAsync(Func<Task> action)
    {
        Exception? exception = default;

        var sw = Stopwatch.StartNew();
        try
        {
            await action.Invoke();
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            sw.Stop();
        }

        return (Success: exception == default, sw.Elapsed, Exception: exception);
    }

    public async static Task<(bool Success, T? Result, TimeSpan Elapsed, Exception? Exception)> TryRunAsync<T>(Func<Task<T>> action)
    {
        T? result = default;
        Exception? exception = default;

        var sw = Stopwatch.StartNew();
        try
        {
            result = await action.Invoke();
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            sw.Stop();
        }

        return (Success: exception == default, Result: result, sw.Elapsed, Exception: exception);
    }
}
