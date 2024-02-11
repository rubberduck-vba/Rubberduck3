using System;
using System.Diagnostics;

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
}
