using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Common
{
    public static class TimedAction
    {
        public static TimeSpan Run(Action action)
        {
            var sw = Stopwatch.StartNew();
            action.Invoke();
            sw.Stop();
            return sw.Elapsed;
        }

        public static async Task<TimeSpan> RunAsync(Task task)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await task;
            }
            finally
            {
                sw.Stop();
            }
            return sw.Elapsed;
        }
    }
}
